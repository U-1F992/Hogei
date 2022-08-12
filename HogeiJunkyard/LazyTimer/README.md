# LazyTimer

開始後に終了時刻を設定できるタイマー

## Usage

```csharp
using System.Diagnostics;
using HogeiJunkyard;

var lazyTimer = new LazyTimer();
var stopwatch = new Stopwatch();

for (var i = 0; i < 3; i++)
{
    Console.WriteLine("---take {0}---", i + 1);
    Console.WriteLine("[start] stopwatch");
    stopwatch.Start();
    
    Console.WriteLine("[start] lazyTimer");
// ~~~ このStartから ~~~
    var timer = lazyTimer.Start().ContinueWith(task =>
    {
        // 終了時の動作の書き方
        Console.WriteLine("[timeout] at {0}", stopwatch.ElapsedMilliseconds);
    });

    // 何らかの処理
    // Submitより時間がかかると、awaitしたtimerは即座に終了します。
    await Task.Delay(2000);

    Console.WriteLine("[submit] at {0}", stopwatch.ElapsedMilliseconds);
    lazyTimer.Submit(5000);

    await timer;
// ~~~ ここまでが、Submitした5000msになる ~~~

    stopwatch.Reset();
}
```