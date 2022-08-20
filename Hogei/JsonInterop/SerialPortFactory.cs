using System.IO.Ports;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Hogei;

public struct SerialPortFactory
{
    [JsonInclude]
    [JsonPropertyName("portName")]
    public string PortName { get; private set; }
    //
    // 省略可能
    //
    [JsonInclude]
    [JsonPropertyName("baudRate")]
    public int? BaudRate { get; private set; }
    [JsonInclude]
    [JsonPropertyName("parity")]
    public int? Parity { get; private set; }
    [JsonInclude]
    [JsonPropertyName("dataBits")]
    public int? DataBits { get; private set; }
    [JsonInclude]
    [JsonPropertyName("stopBits")]
    public int? StopBits { get; private set; }
    [JsonInclude]
    [JsonPropertyName("dtrEnable")]
    public bool? DtrEnable { get; private set; }
    [JsonInclude]
    [JsonPropertyName("rtsEnable")]
    public bool? RtsEnable { get; private set; }
    [JsonInclude]
    [JsonPropertyName("encoding")]
    public string? Encoding { get; private set; }
    [JsonInclude]
    [JsonPropertyName("newLine")]
    public string? NewLine { get; private set; }

    public static SerialPort FromJson(string path)
    {
        return JsonSerializer.Deserialize<SerialPortFactory>(File.ReadAllText(path)).CreateInstance();
    }

    SerialPort CreateInstance()
    {
        var serialPort = new SerialPort(PortName);

        // 省略可能なプロパティの処理
        serialPort.BaudRate = BaudRate == null ? serialPort.BaudRate : (int)BaudRate;
        serialPort.Parity = Parity == null ? serialPort.Parity : (Parity)Parity;
        serialPort.DataBits = DataBits == null ? serialPort.DataBits : (int)DataBits;
        serialPort.StopBits = StopBits == null ? serialPort.StopBits : (StopBits)StopBits;
        serialPort.DtrEnable = DtrEnable == null ? serialPort.DtrEnable : (bool)DtrEnable;
        serialPort.RtsEnable = RtsEnable == null ? serialPort.RtsEnable : (bool)RtsEnable;
        serialPort.NewLine = NewLine == null ? serialPort.NewLine : (string)NewLine;
        
        // EncodingオブジェクトはJSONで交換できない
        var encoding = serialPort.Encoding;
        switch (Encoding)
        {
            case "ASCII":
                encoding = System.Text.Encoding.ASCII;
                break;
            case "BigEndianUnicode":
                encoding = System.Text.Encoding.BigEndianUnicode;
                break;
            case "Latin1":
                encoding = System.Text.Encoding.Latin1;
                break;
            case "Unicode":
                encoding = System.Text.Encoding.Unicode;
                break;
            case "UTF32":
                encoding = System.Text.Encoding.UTF32;
                break;
            // case "UTF7":
            //     encoding = System.Text.Encoding.UTF7;
            //     break;
            case "UTF8":
                encoding = System.Text.Encoding.UTF8;
                break;
                
            default:
                break;
        }
        serialPort.Encoding = encoding;

        return serialPort;
    }
}
