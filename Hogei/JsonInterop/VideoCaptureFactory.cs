using System.Text.Json;
using System.Text.Json.Serialization;
using OpenCvSharp;
namespace Hogei;

public struct VideoCaptureFactory
{
    [JsonInclude]
    [JsonPropertyName("index")]
    public int Index { get; private set; }
    //
    // 省略可能
    //
    [JsonInclude]
    [JsonPropertyName("frameWidth")]
    public int? FrameWidth { get; private set; }
    [JsonInclude]
    [JsonPropertyName("frameHeight")]
    public int? FrameHeight { get; private set; }
    [JsonInclude]
    [JsonPropertyName("apiPreference")]
    public int? ApiPreference { get; private set; }
    
    public static VideoCapture FromJson(string path)
    {
        return JsonSerializer.Deserialize<VideoCaptureFactory>(File.ReadAllText(path)).CreateInstance();
    }

    VideoCapture CreateInstance()
    {
        // コンストラクタでしか変更できない省略可能な引数は、オブジェクト初期化の前に処理
        var apiPreference = ApiPreference == null ? VideoCaptureAPIs.ANY : (VideoCaptureAPIs)ApiPreference;
        
        var videoCapture = new VideoCapture(Index, apiPreference);

        // 省略可能なプロパティの処理
        videoCapture.FrameWidth = FrameWidth == null ? videoCapture.FrameWidth : (int)FrameWidth;
        videoCapture.FrameHeight = FrameHeight == null ? videoCapture.FrameHeight : (int)FrameHeight;

        return videoCapture;
    }
}