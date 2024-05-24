using System.Drawing;
using Yolov5Net.Scorer.Models.Abstract;

namespace Yolov5Net.Scorer.Models;

public record YoloCocoP5Model_NanyaPU() : YoloModel(
    640,
    640,
    3,

    10,

    new[] { 8, 16, 32 },

    new[]
    {
        new[] { new[] { 010, 13 }, new[] { 016, 030 }, new[] { 033, 023 } },
        new[] { new[] { 030, 61 }, new[] { 062, 045 }, new[] { 059, 119 } },
        new[] { new[] { 116, 90 }, new[] { 156, 198 }, new[] { 373, 326 } }
    },

    new[] { 80, 40, 20 },

    0.25f,
    0.25f,
    0.45f,

    new[] { "output" },

    new()
    {
        new(1, "黑點雜質", Color.Red),
        new(2, "氣泡", Color.Blue),
        new(3, "牽絲", Color.Yellow),
        new(4, "貼合不良", Color.White),
        new(5, "內輪不良", Color.Green)
    },

    true
);
