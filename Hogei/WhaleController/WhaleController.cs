using System.IO.Ports;
using System.Text;
using NLog;

namespace Hogei;
public class WhaleController
{
    static Logger logger = LogManager.GetCurrentClassLogger();

    SerialPort serialPort;

    string newline;
    string buffer = "";
    object lockObject = new Object();

    public WhaleController(SerialPort serialPort)
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

        // シリアルポートからの出力をログに書く
        this.serialPort.DataReceived += (object sender, SerialDataReceivedEventArgs eventArgs) =>
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

    public void Run(ICollection<Operation> sequence)
    {
        foreach (var operation in sequence)
        {
            Run(operation);
        }
    }
    void Run(Operation operation)
    {
        Task.WaitAll
        (
            Task.Run(() =>
            {
                foreach (var key in operation.Keys)
                {
                    this.serialPort.WriteLine(((char)key).ToString());
                }
            }),
            Task.Delay(operation.Wait)
        );
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
