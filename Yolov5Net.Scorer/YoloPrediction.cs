using SixLabors.ImageSharp;

namespace Yolov5Net.Scorer;

/// <summary>
/// Object prediction.
/// </summary>
//public record YoloPrediction(YoloLabel Label, float Score, RectangleF Rectangle);
public record YoloPrediction
{
    public YoloLabel Label { get; init; }
    public float Score { get; init; }
    public RectangleF Rectangle { get; init; }
    public YoloPrediction(YoloLabel label, float score, RectangleF rectangle)
    {
        Label = label;
        Score = score;
        Rectangle = rectangle;
    }

    public YoloPrediction()
    {
    }
}

