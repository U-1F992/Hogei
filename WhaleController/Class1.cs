using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Text;

namespace HogeiJunkyard;
public class WhaleController
{
    SerialPort serialPort;
    ConcurrentQueue<Operation> concurrentQueue = new();
    Task dequeue;

    public WhaleController(SerialPort serialPort)
    {
        this.serialPort = serialPort;
        dequeue = Task.Run(async () =>
        {
            while (true)
            {
                if (concurrentQueue.TryDequeue(out Operation? operation))
                {
                    if (operation == null)
                    {
                        break;
                    }
                    await Task.WhenAll(
                        Task.Run(() =>
                        {
                            foreach (var key in operation.Key)
                            {
                                var buffer = Encoding.ASCII.GetBytes(new char[] { (char)key, '\n' });
                                serialPort.BaseStream.WriteAsync(buffer);
                            }
                        }),
                        Task.Delay(operation.Wait)
                    );
                }
            }
        });
    }
}

public record Operation
{
    private KeySpecEnum[] _Key;
    public IReadOnlyCollection<KeySpecEnum> Key
    {
        get
        {
            return _Key;
        }
    }
    public readonly TimeSpan Wait;
    public Operation(ICollection<KeySpecEnum> key, TimeSpan wait)
    {
        _Key = key.ToArray();
        Wait = wait;
    }
}

public enum KeySpecEnum
{
    A = 'a',
    B = 'b'
}
