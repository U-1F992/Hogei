# VideoCaptureWrapper

別スレッドでの常時Mat取得と、プレビュー表示を行うラッパークラス

## Usage

利用側で`OpenCvSharp4.runtime.*`を追加してください。

```pwsh
dotnet add .\TestApp\ package OpenCvSharp4.runtime.win
```

```csharp
using OpenCvSharp;
using Hogei;

using var videoCapture = new VideoCapture(1)
{
    FrameWidth = 1920,
    FrameHeight = 1080
};

var video = new VideoCaptureWrapper(videoCapture, new Size(960, 540));
```
