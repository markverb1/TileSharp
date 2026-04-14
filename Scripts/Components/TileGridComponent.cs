using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;
using TileSharp.Ecs;

namespace TileSharp.Components;

[GlobalClass]
public partial class TileGridComponent : ComponentBase
{
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Mask : UInt16
    {
        AbsentN = 1 << 0,
        AbsentS = 1 << 1,
        AbsentE = 1 << 2,
        AbsentW = 1 << 3,
        AbsentNE = 1 << 4,
        AbsentSE = 1 << 5,
        AbsentNW = 1 << 6,
        AbsentSW = 1 << 7,
        PresentN = 1 << 8,
        PresentS = 1 << 9,
        PresentE = 1 << 10,
        PresentW = 1 << 11,
        PresentNE = 1 << 12,
        PresentSE = 1 << 13,
        PresentNW = 1 << 14,
        PresentSW = 1 << 15
    }
    public Mask[] DefaultNeighborMapping { get; private set; } =
    [
        Mask.AbsentS | Mask.AbsentW | Mask.AbsentE | Mask.AbsentN,
        Mask.AbsentW | Mask.AbsentN | Mask.PresentE | Mask.PresentS,
        Mask.AbsentE | Mask.AbsentN | Mask.PresentW | Mask.PresentS,
        Mask.AbsentE | Mask.AbsentS | Mask.PresentW | Mask.PresentN,
        Mask.AbsentW | Mask.AbsentS | Mask.PresentE | Mask.PresentN,
        Mask.AbsentW,
        Mask.AbsentS,
        Mask.AbsentE,
        Mask.AbsentN,
        Mask.PresentN | Mask.PresentE | Mask.AbsentNE,
        Mask.PresentS | Mask.PresentE | Mask.AbsentSE,
        Mask.PresentN | Mask.PresentW | Mask.AbsentNW,
        Mask.PresentS | Mask.PresentW | Mask.AbsentSW,
        0
    ];

    public Dictionary<Vector2I, bool> Tiles = new();

    public bool FirstTimeProcessed = false;

    [Export] public TileSet Tileset;
}