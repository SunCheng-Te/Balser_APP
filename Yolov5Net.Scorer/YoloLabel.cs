

using System.Drawing;

namespace Yolov5Net.Scorer;

/// <summary>
/// Label of detected object.
/// </summary>
public record YoloLabel(int Id, string Name, Color Color, YoloLabelKind Kind)
{
    public YoloLabel(int id, string name, Color color) : this(id, name, color, YoloLabelKind.Generic) { }
}
