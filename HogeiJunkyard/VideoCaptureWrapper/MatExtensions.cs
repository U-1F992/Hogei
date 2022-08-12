namespace HogeiJunkyard;

using OpenCvSharp;
using OpenCvSharp.Text;

public static class MatExtensions
{
    /// <summary>
    /// 大きい方の画像に小さい方の画像が含まれているか調べる。
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="source"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    public static bool Contains(this Mat mat, Mat source, double threshold = 0.75)
    {
        double result;
        if (mat.Width >= source.Width && mat.Height >= source.Height)
            // matの各辺の長さがそれぞれsource以上の場合
            result = MatchTemplate(mat, source);
        else if (mat.Width <= source.Width && mat.Height <= source.Height)
            // sourceの各辺の長さがそれぞれmat以上の場合
            result = MatchTemplate(source, mat);
        else
            // 一方をもう一方に収めることができない場合
            throw new Exception("It doesn't fit either.");

        return result >= threshold;

        double MatchTemplate(Mat larger, Mat smaller)
        {
            using (var result = larger.MatchTemplate(smaller, TemplateMatchModes.CCoeffNormed))
            {
                result.MinMaxLoc(out double minVal, out double maxVal);
                return maxVal;
            }
        }
    }

    /// <summary>
    /// Streamに変換する。
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="fileName">生成した中間ファイル名<br/>必要に応じて削除する</param>
    /// <returns></returns>
    public static Stream ToStream(this Mat mat, out string fileName)
    {
        var tmpPath = Path.GetTempFileName();
        fileName = Path.Join(Path.GetDirectoryName(tmpPath), Path.GetFileNameWithoutExtension(tmpPath) + ".png");
        mat.SaveImage(fileName);

        return File.OpenRead(fileName);
    }

    /// <summary>
    /// OCR結果を取得する。
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="tessConfig">datapath, language, charWhitelist, oem, psmodeを設定可能</param>
    /// <returns></returns>
    public static string GetOCRResult(this Mat mat, TessConfig tessConfig)
    {
        using (var tesseract = OCRTesseract.Create(tessConfig.DataPath, tessConfig.Language, tessConfig.CharWhitelist, tessConfig.Oem, tessConfig.PsMode))
        {
            tesseract.Run(mat, out var outputText, out var componentRects, out var componentTexts, out var componentConfidences);
            return outputText;
        }
    }
}
