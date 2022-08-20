namespace Hogei;

using System.Diagnostics;
using OpenCvSharp;
using NLog;

public class Preview
{
    static Logger logger = LogManager.GetCurrentClassLogger();

    VideoCapture videoCapture;
    Mat mat = new Mat();
    Task task;
    
    public delegate Mat ProcessHandler(Mat mat);
    public event ProcessHandler Process = mat => { return mat; };

    public Preview(VideoCapture videoCapture) : this(videoCapture, Size.Zero, null) { }
    public Preview(VideoCapture videoCapture, Size windowSize) : this(videoCapture, windowSize, null) { }
    public Preview(VideoCapture videoCapture, Size windowSize, string? windowName)
    {
        this.videoCapture = videoCapture;

        // 接続からtimeoutで初回Matを取得できなかった場合throw
        // 不正な画像を渡すデバイスに対するフリーズ防止
        var ready = false;
        var stopwatch = new Stopwatch();
        var timeout = TimeSpan.FromMilliseconds(5000);

        task = Task.WhenAll
        (
            Task.Run(() =>
            {
                // matを更新するTask
                while (!this.videoCapture.IsOpened()) ;

                stopwatch.Start();
                while (true)
                {
                    bool grab;
                    lock (mat)
                    {
                        grab = this.videoCapture.Read(mat);
                    }
                    if (grab && !ready)
                    {
                        ready = true;
                    }
                }
            }),
            Task.Run(() =>
            {
                // プレビューを表示するタスク
                if (windowSize == Size.Zero)
                {
                    return;
                }
                while (!ready)
                {
                    Thread.Sleep(1);
                }

                using var window = (windowName == null ? new Window() : new Window(windowName));
                while (true)
                {
                    try
                    {
                        using var raw = CurrentFrame;
                        using var processed = Process(raw);
                        using var resized = raw.Resize(windowSize);
                        window.ShowImage(resized);
                    }
                    catch (Exception e)
                    {
                        logger.Warn(e);
                    }

                    if (Cv2.WaitKey(1) == (int)'s')
                    {
                        var fileName = Path.Combine(AppContext.BaseDirectory, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png");

                        using var save = CurrentFrame;
                        save.SaveImage(fileName);

                        logger.Info("The image was saved to {0}", fileName);
                    }
                }
            })
        );
        while (!ready && stopwatch.Elapsed < timeout)
        {
            Thread.Sleep(1);
        }
        if (!ready)
        {
            throw new Exception("VideoCapture seems not to open.");
        }
    }

    public Mat CurrentFrame
    {
        get
        {
            try
            {
                return mat.Clone();
            }
            catch (Exception e)
            {
                logger.Warn(e);
                return new Mat(new Size(videoCapture.FrameWidth, videoCapture.FrameHeight), MatType.CV_8UC3);
            }
        }
    }
}
