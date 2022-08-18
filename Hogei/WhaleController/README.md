# WhaleController

[WHALE](https://github.com/mizuyoukanao/WHALE)を書き込んだマイコンを制御する

## Usage

```csharp
using System.IO.Ports;
using System.Text;
using HogeiJunkyard;

// SerialPortの宣言方法と設定
using var serialPort = new SerialPort("COM6", 4800)
{
    Encoding = Encoding.UTF8,
    NewLine = "\r\n",
    DtrEnable = true,
    RtsEnable = true
};
serialPort.Open();
var whale = new WhaleController(serialPort);

whale.Run(new Operation[]
{
    new Operation(new KeySpecifier[] { KeySpecifier.Start_Down }, TimeSpan.FromMilliseconds(500)),
    new Operation(new KeySpecifier[] { KeySpecifier.Start_Up }, TimeSpan.FromMilliseconds(500)),
    new Operation(new KeySpecifier[] { KeySpecifier.B_Down }, TimeSpan.FromMilliseconds(500)),
    new Operation(new KeySpecifier[] { KeySpecifier.B_Up }, TimeSpan.FromMilliseconds(500)),
});
```
