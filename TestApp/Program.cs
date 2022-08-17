using System.IO.Ports;
using System.Text;
using OpenCvSharp;
using HogeiJunkyard;

var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;

using var serialPort = new SerialPort("COM6", 4800)
{
    Encoding = Encoding.UTF8,
    NewLine = "\r\n",
    DtrEnable = true,
    RtsEnable = true
};
serialPort.Open();
var whale = new WhaleController(serialPort);

using var videoCapture = new VideoCapture(1)
{
    FrameWidth = 1920,
    FrameHeight = 1080
};
var video = new VideoCaptureWrapper(videoCapture, new Size(960, 540));

await Task.Delay(1000);
showFourTimes(whale);

void showFourTimes(WhaleController whale)
{
    var stopwatch = new System.Diagnostics.Stopwatch();
    var Key_A_Down = new KeySpecifier[] { KeySpecifier.A_Down };
    var Key_A_Up = new KeySpecifier[] { KeySpecifier.A_Up };
    var standardDuration = TimeSpan.FromMilliseconds(200);

    var sequence = new Operation[]
    {
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(9000)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(1500)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(5000)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(4000)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(11500)),

        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(1500)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(1500)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(1500)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(59000)),

        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(1500)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(1500)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(1500)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(3000)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(1000)),
        new Operation(new KeySpecifier[] { KeySpecifier.Start_Down }, TimeSpan.FromMilliseconds(1000)),
        new Operation(new KeySpecifier[] { KeySpecifier.Start_Up }, TimeSpan.FromMilliseconds(1000)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(7000)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(2000)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(2000)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(2000)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(32000)),

        new Operation(new KeySpecifier[] { KeySpecifier.X_Down }, standardDuration),
        new Operation(new KeySpecifier[] { KeySpecifier.X_Up }, TimeSpan.FromMilliseconds(3000)),
        new Operation(new KeySpecifier[] { KeySpecifier.Right_Down }, standardDuration),
        new Operation(new KeySpecifier[] { KeySpecifier.Right_Up }, TimeSpan.FromMilliseconds(2000)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(5000))
    };
    var reset = new Operation[]
    {
        new Operation(new KeySpecifier[] { KeySpecifier.L_Down, KeySpecifier.R_Down, KeySpecifier.Z_Down, KeySpecifier.Start_Down }, TimeSpan.FromMilliseconds(2000)),
        new Operation(new KeySpecifier[] { KeySpecifier.L_Up, KeySpecifier.R_Up, KeySpecifier.Z_Up, KeySpecifier.Start_Up }, TimeSpan.FromMilliseconds(4000)),
        new Operation(new KeySpecifier[] { KeySpecifier.X_Down }, standardDuration),
        new Operation(new KeySpecifier[] { KeySpecifier.X_Up }, TimeSpan.FromMilliseconds(1000)),
        new Operation(Key_A_Down, standardDuration),
        new Operation(Key_A_Up, TimeSpan.FromMilliseconds(5000)),
    };

    stopwatch.Start();
    var count = 0;
    var timer = new Timer(_ =>
    {
        if (count > 3)
        {
            return;
        }

        Console.WriteLine("{0}: {1}", count, stopwatch.ElapsedMilliseconds);
        whale.Run(sequence);
        using var frame = video.CurrentFrame;
        frame.SaveImage(string.Format("{0}.png", count));
        whale.Run(reset);
        
        Interlocked.Increment(ref count);
    }, null, TimeSpan.Zero, TimeSpan.FromMinutes(3));
    Thread.Sleep(TimeSpan.FromMinutes(3 * 4));
}
