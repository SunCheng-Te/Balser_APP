using System.Drawing;
using Yolov5Net.Scorer.Models.Abstract;

namespace Yolov5Net.Scorer.Models;

public record YoloCocoP6Model() : YoloModel(
    1280,
    1280,
    3,

    8,

    new[] { 8, 16, 32, 64 },

    new[]
    {
        new[] { new[] { 019, 027 }, new[] { 044, 040 }, new[] { 038, 094 } },
        new[] { new[] { 096, 068 }, new[] { 086, 152 }, new[] { 180, 137 } },
        new[] { new[] { 140, 301 }, new[] { 303, 264 }, new[] { 238, 542 } },
        new[] { new[] { 436, 615 }, new[] { 739, 380 }, new[] { 925, 792 } }
    },

    new[] { 160, 80, 40, 20 },

    0.20f,
    0.25f,
    0.45f,

    new[] { "output" },

    new()
    {
        new(1, "CircleGuage", Color.Red),
        new(2, "ArcGauge", Color.Blue),
        new(3, "7Seg", Color.Yellow)
    },

    true
);
