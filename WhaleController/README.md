# WhaleController

[WHALE](https://github.com/mizuyoukanao/WHALE)を書き込んだマイコンを制御する

## Usage

```csharp
// SerialPortの宣言方法と設定
using var serialPort = new SerialPort("COM6", 4800)
{
    Encoding = Encoding.UTF8,
    NewLine = "\r\n",
    DtrEnable = true,
    RtsEnable = true
};
serialPort.Open();
var whale = new WhaleController(serialPort, cancellationToken);

// キューに詰めた操作は、非同期で順次実行されます。
whale.Enqueue(new Operation[]
{
    new Operation(new KeySpecifier[] { KeySpecifier.Right_Down }, TimeSpan.FromMilliseconds(500)),
    new Operation(new KeySpecifier[] { KeySpecifier.Right_Up }, TimeSpan.FromMilliseconds(500)),
    new Operation(new KeySpecifier[] { KeySpecifier.Left_Down }, TimeSpan.FromMilliseconds(500)),
    new Operation(new KeySpecifier[] { KeySpecifier.Left_Up }, TimeSpan.FromMilliseconds(500)),
});

// Runメソッドを使用すると、キューとは関係なく、即座に実行することもできます。
await whale.Run(new Operation[]
{
    new Operation(new KeySpecifier[] { KeySpecifier.Start_Down }, TimeSpan.FromMilliseconds(500)),
    new Operation(new KeySpecifier[] { KeySpecifier.Start_Up }, TimeSpan.FromMilliseconds(500)),
    new Operation(new KeySpecifier[] { KeySpecifier.B_Down }, TimeSpan.FromMilliseconds(500)),
    new Operation(new KeySpecifier[] { KeySpecifier.B_Up }, TimeSpan.FromMilliseconds(500)),
}, cancellationToken);

// キューが空になるまで待機します。
whale.WaitForDequeue();
```
