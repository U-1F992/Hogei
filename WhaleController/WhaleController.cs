﻿using System.Collections.Concurrent;
using System.IO.Ports;
using System.Text;
using NLog;

namespace HogeiJunkyard;
public class WhaleController
{
    static Logger logger = LogManager.GetCurrentClassLogger();

    SerialPort serialPort;
    ConcurrentQueue<Operation> concurrentQueue = new();
    Task dequeue;

    string newline;
    string buffer = "";
    object lockObject = new Object();


    public WhaleController(SerialPort serialPort) : this(serialPort, CancellationToken.None) { }
    public WhaleController(SerialPort serialPort, CancellationToken cancellationToken)
    {
        if (serialPort.Encoding != Encoding.UTF8)
        {
            throw new Exception("serialPort.Encoding must be Encoding.UTF8");
        }
        if (serialPort.RtsEnable != true)
        {
            throw new Exception("serialPort.RtsEnable must be true");
        }
        if (serialPort.DtrEnable != true)
        {
            throw new Exception("serialPort.DtrEnable must be true");
        }

        this.serialPort = serialPort;
        newline = serialPort.NewLine;

        // キューを順次シリアルポートに流すTask
        dequeue = Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!this.serialPort.IsOpen)
                {
                    continue;
                }
                if (!concurrentQueue.TryDequeue(out Operation? operation))
                {
                    continue;
                }
                if (operation == null)
                {
                    continue;
                }

                logger.Trace("Dequeue: {0}", JoinEnumCollection(operation.Keys));
                await Run(operation);
            }
        }, cancellationToken);

        // シリアルポートからの出力をログに書く
        this.serialPort.DataReceived += (object sender, SerialDataReceivedEventArgs e) =>
        {
            var serialPort = (SerialPort)sender;
            if (!serialPort.IsOpen)
            {
                return;
            }

            // bufferに追加する
            var message = serialPort.ReadExisting();
            lock (lockObject)
            {
                buffer += message;
            }

            var split = buffer.Split(newline);
            if (split.Length <= 1)
            {
                return;
            }
            // 1行目を書く
            var toWrite = split[0];
            logger.Trace(toWrite);

            // 残りはbufferに返す
            lock (lockObject)
            {
                buffer = buffer[(toWrite.Length + newline.Length)..];
            }
        };
    }

    public void Enqueue(ICollection<Operation> sequence)
    {
        foreach (var operation in sequence)
        {
            concurrentQueue.Enqueue(operation);
            logger.Trace("Enqueue: {0}", JoinEnumCollection(operation.Keys));
        }
    }

    public async Task Run(ICollection<Operation> sequence)
    {
        foreach (var operation in sequence)
        {
            await Run(operation);
        }
    }
    public async Task Run(Operation operation)
    {
        logger.Trace("Run: {0}", JoinEnumCollection(operation.Keys));
        await Task.WhenAll(
            Task.Run(() =>
            {
                foreach (var key in operation.Keys)
                {
                    // https://www.arduino.cc/reference/en/language/functions/communication/serial/println/
                    var buffer = (new char[] { (char)key }).Concat(newline.ToCharArray()).ToArray();
                    var bytes = Encoding.UTF8.GetBytes(buffer);

                    this.serialPort.BaseStream.WriteAsync(bytes);
                }
            }),
            Task.Delay(operation.Wait)
        );
    }
    public void WaitForDequeue()
    {
        logger.Trace("Wait for dequeue...");
        while (!concurrentQueue.IsEmpty)
        {
            Thread.Sleep(1);
        }
        logger.Trace("Dequeue completed");
    }
    string JoinEnumCollection(IReadOnlyCollection<KeySpecifier> keys, string separator = ",")
    {
        var join = "";
        foreach (var key in keys)
        {
            join += key + separator;
        }
        return join.Remove(join.Length - separator.Length);
    }
}
