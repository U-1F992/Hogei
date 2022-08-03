using System.IO.Ports;
using System.Text;
using HogeiJunkyard;

using var serialPort = new SerialPort("COM6", 4800)
{
    Encoding = Encoding.UTF8,
    NewLine = "\r\n",
    DtrEnable = true,
    RtsEnable = true
};
serialPort.Open();
var whale = new WhaleController(serialPort);
await Task.Delay(500);

for (var i = 0; i < 10; i++)
{
    whale.Enqueue(new Operation[]
    {
        new Operation(new KeySpecifier[] { KeySpecifier.Right_Down }, TimeSpan.FromMilliseconds(500)),
        new Operation(new KeySpecifier[] { KeySpecifier.Right_Up }, TimeSpan.FromMilliseconds(500)),
        new Operation(new KeySpecifier[] { KeySpecifier.Left_Down }, TimeSpan.FromMilliseconds(500)),
        new Operation(new KeySpecifier[] { KeySpecifier.Left_Up }, TimeSpan.FromMilliseconds(500)),
    });
}
await Task.Delay(3000).ContinueWith(async task => 
{
    await whale.Run(new Operation[]
    {
        new Operation(new KeySpecifier[] { KeySpecifier.Start_Down }, TimeSpan.FromMilliseconds(500)),
        new Operation(new KeySpecifier[] { KeySpecifier.Start_Up }, TimeSpan.FromMilliseconds(500)),
        new Operation(new KeySpecifier[] { KeySpecifier.B_Down }, TimeSpan.FromMilliseconds(500)),
        new Operation(new KeySpecifier[] { KeySpecifier.B_Up }, TimeSpan.FromMilliseconds(500)),
    });
});

whale.WaitForDequeue();

