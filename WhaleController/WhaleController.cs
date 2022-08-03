using System.Collections.Concurrent;
using System.IO.Ports;
using System.Text;

namespace HogeiJunkyard;
public class WhaleController
{
    SerialPort serialPort;
    ConcurrentQueue<Operation> concurrentQueue = new();
    Task dequeue;

    public WhaleController(SerialPort serialPort) : this(serialPort, CancellationToken.None) { }
    public WhaleController(SerialPort serialPort, CancellationToken cancellationToken)
    {
        this.serialPort = serialPort;
        dequeue = Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!serialPort.IsOpen)
                {
                    continue;
                }

                if (concurrentQueue.TryDequeue(out Operation? operation))
                {
                    continue;
                }

                if (operation == null)
                {
                    continue;
                }

                await Task.WhenAll(
                    Task.Run(() =>
                    {
                        foreach (var key in operation.Keys)
                        {
                            var buffer = Encoding.ASCII.GetBytes(new char[] { (char)key, '\n' });
                            serialPort.BaseStream.WriteAsync(buffer);
                        }
                    }),
                    Task.Delay(operation.Wait)
                );
            }
        }, cancellationToken);
    }
}
