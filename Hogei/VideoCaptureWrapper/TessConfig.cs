namespace Hogei;

using System.Text.Json.Serialization;
public readonly struct TessConfig
{
    public TessConfig(string? datapath = null, string? language = null, string? charWhitelist = null, int oem = 3, int psmode = 3)
    {
        if (datapath != null && !Directory.Exists(datapath))
        {
            throw new Exception("DataPath must be an existing directory.");
        }
        if (datapath != null && language != null && !File.Exists(Path.Join(datapath, string.Format("{0}.traineddata", language))))
        {
            throw new Exception(string.Format("Language \"{0}\" is not installed.", language));
        }
        if (oem < 0 || 3 < oem)
        {
            throw new Exception("Oem must be between 0 and 3. https://github.com/tesseract-ocr/tesseract/blob/main/doc/tesseract.1.asc#options");
        }
        if (psmode < 0 || 13 < psmode)
        {
            throw new Exception("PsMode must be between 0 and 13. https://github.com/tesseract-ocr/tesseract/blob/main/doc/tesseract.1.asc#options");
        }

        DataPath = datapath;
        Language = language;
        CharWhitelist = charWhitelist;
        Oem = oem;
        PsMode = psmode;
    }
    [JsonPropertyName("datapath")]
    public readonly string? DataPath { init; get; }
    [JsonPropertyName("language")]
    public readonly string? Language { init; get; }
    [JsonPropertyName("charWhitelist")]
    public readonly string? CharWhitelist { init; get; }
    [JsonPropertyName("oem")]
    public readonly int Oem { init; get; }
    [JsonPropertyName("psmode")]
    public readonly int PsMode { init; get; }
}
