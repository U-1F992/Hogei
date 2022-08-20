using OpenCvSharp;
using Hogei;

var GetFullPath = (string fileName) => Path.Join(AppContext.BaseDirectory, fileName);

using var serialPort = SerialPortFactory.FromJson(GetFullPath("serialport.config.json"));
serialPort.Open();
var whale = new Whale(serialPort);

using var videoCapture = VideoCaptureFactory.FromJson(GetFullPath("videocapture.config.json"));
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

showFourTimes(whale, preview);

void showFourTimes(Whale whale, Preview preview)
{
    Thread.Sleep(1000);
    var stopwatch = new System.Diagnostics.Stopwatch();

    var sequences = Operation.GetDictionaryFromJson(GetFullPath("sequences.json"));
    var load = sequences["load"];
    var reset = sequences["reset"];

    stopwatch.Start();
    var count = 0;
    var timer = new Timer(_ =>
    {
        if (count > 3)
        {
            return;
        }

        Console.WriteLine("{0}: {1}", count, stopwatch.ElapsedMilliseconds);
        whale.Run(load);
        using var frame = preview.CurrentFrame;
        frame.SaveImage(string.Format("{0}.png", count));
        whale.Run(reset);
        
        Interlocked.Increment(ref count);
    }, null, TimeSpan.Zero, TimeSpan.FromMinutes(3));

    Thread.Sleep(TimeSpan.FromMinutes(3 * 4));
}
