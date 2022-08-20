using OpenCvSharp;
using Hogei;

var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;

using var serialPort = SerialPortFactory.FromJson(Path.Join(AppContext.BaseDirectory, "serialport.config.json"));
if (serialPort.DtrEnable != true)
{
    Console.Error.WriteLine("assertion failed");
    return;
}
serialPort.Open();
var whale = new Whale(serialPort);

using var videoCapture = VideoCaptureFactory.FromJson(Path.Join(AppContext.BaseDirectory, "videocapture.config.json"));
var preview = new Preview(videoCapture, new Size(960, 540));
var prev = DateTime.Now;
preview.Process += mat =>
{
    Cv2.PutText(mat, DateTime.Now.ToString(), new Point(100, 100), HersheyFonts.HersheySimplex, 1, Scalar.White, 3);
    
    var now = DateTime.Now;
    Cv2.PutText(mat, string.Format("fps: {0:F4}", 1000 / (now - prev).TotalMilliseconds), new Point(100, 150), HersheyFonts.HersheySimplex, 1, Scalar.White, 3);
    prev = now;

    return mat;
};

await Task.Delay(1000);
showFourTimes(whale);

void showFourTimes(Whale whale)
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
        using var frame = preview.CurrentFrame;
        frame.SaveImage(string.Format("{0}.png", count));
        whale.Run(reset);
        
        Interlocked.Increment(ref count);
    }, null, TimeSpan.Zero, TimeSpan.FromMinutes(3));
    Thread.Sleep(TimeSpan.FromMinutes(3 * 4));
}
