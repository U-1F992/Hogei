namespace Hogei;
public class LazyTimer
{
    Task task = Task.CompletedTask;
    TimeSpan elapsed = TimeSpan.Zero;
    TimeSpan submitted;

    public LazyTimer() : this(TimeSpan.MaxValue) { }
    public LazyTimer(TimeSpan timeSpan)
    {
        submitted = timeSpan;

        // 初回のみ遅延があるため、コンストラクタで捨てておく
        // メモリに展開する際の何らかな気がする
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        cancellationTokenSource.Cancel();

        try
        {
            Start(cancellationToken).Wait(cancellationToken);
        }
        catch (OperationCanceledException) { /* Successfully cancelled */ }
    }

    public async Task Start() { await Start(CancellationToken.None); }
    public async Task Start(CancellationToken cancellationToken)
    {
        elapsed = TimeSpan.Zero;

        task = Task.Run(() =>
        {
            var interval = 10000000 / 1000;
            var next = DateTime.Now.Ticks + interval;

            while (elapsed < submitted && !cancellationToken.IsCancellationRequested)
            {
                if (next > DateTime.Now.Ticks)
                {
                    continue;
                }
                elapsed = TimeSpan.FromMilliseconds(elapsed.TotalMilliseconds + 1);
                next += interval;
            }
        }, cancellationToken);
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            throw;
        }

        elapsed = TimeSpan.Zero;
        submitted = TimeSpan.MaxValue;
    }

    public void Submit(TimeSpan timeSpan)
    {
        submitted = timeSpan;
    }
}
