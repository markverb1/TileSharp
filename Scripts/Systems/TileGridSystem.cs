using System;
using System.Collections.Generic;
using Godot;
using TileSharp.Components;
using TileSharp.Ecs;

namespace TileSharp.Systems;

[GlobalClass]
public partial class TileGridSystem : SystemBase
{
    public Node2D TilesParent { get; private set; }
    protected override List<Type> WhitelistedTypes { get; } = [typeof(TileGridComponent)];
    protected override List<Type> BlacklistedTypes { get; } = [];
    private readonly List<TileMapLayer> _layers = [];

    protected override void _SystemReady()
    {
        TilesParent = new Node2D();
        TilesParent.Name = "TilesParent";
        World.Viewport.AddChild(TilesParent);

        var tgComponent = new TileGridComponent();
        var idx = 0;

        foreach (var layer in tgComponent.DefaultNeighborMapping)
        {
            var tileLayer = new TileMapLayer();
            _layers.Add(tileLayer);
            tileLayer.Name = idx++.ToString();
            TilesParent.AddChild(tileLayer);
            TilesParent.MoveChild(tileLayer, 0);
        }
    }

    public override void _Process(double delta)
    {
        foreach (var entity in Entities)
        {
            //GD.Print(entity.EntityName);
            var tgComponent = entity.GetComponent<TileGridComponent>();
            if (!tgComponent.FirstTimeProcessed)
            {
                tgComponent.Tiles.Add(new Vector2I(1, 1), true);
                tgComponent.Tiles.Add(new Vector2I(1, 2), true);
                tgComponent.Tiles.Add(new Vector2I(1, 3), true);
                tgComponent.Tiles.Add(new Vector2I(2, 1), true);
                //tgComponent.Tiles.Add(new Vector2I(2, 2), true);
                tgComponent.Tiles.Add(new Vector2I(2, 3), true);
                tgComponent.Tiles.Add(new Vector2I(3, 1), true);
                tgComponent.Tiles.Add(new Vector2I(3, 2), true);
                tgComponent.Tiles.Add(new Vector2I(3, 3), true);
                foreach (var layer in _layers) layer.TileSet = tgComponent.Tileset;
                tgComponent.FirstTimeProcessed = true;
            }

            foreach (var tile in tgComponent.Tiles)
            {
                if (!tile.Value) continue;
                var bitmask = GetBitMask(tgComponent.Tiles, tile.Key);
                int layerIdx = 0;
                foreach (var layer in _layers)
                {
                    if (bitmask.HasFlag(tgComponent.DefaultNeighborMapping[layerIdx]))
                        layer.SetCell(tile.Key, 2, new Vector2I(layerIdx % 7, layerIdx / 7));
                    layerIdx++;
                }
            }
        }
    }

    TileGridComponent.Mask GetBitMask(Dictionary<Vector2I, bool> tiles, Vector2I origin)
    {
        TileGridComponent.Mask mask = 0;

        if (GetNeighbor(tiles, origin, new Vector2I(0, -1))) mask |= TileGridComponent.Mask.PresentN;
        else mask |= TileGridComponent.Mask.AbsentN;
        if (GetNeighbor(tiles, origin, new Vector2I(0, 1))) mask |= TileGridComponent.Mask.PresentS;
        else mask |= TileGridComponent.Mask.AbsentS;
        if (GetNeighbor(tiles, origin, new Vector2I(1, 0))) mask |= TileGridComponent.Mask.PresentE;
        else mask |= TileGridComponent.Mask.AbsentE;
        if (GetNeighbor(tiles, origin, new Vector2I(-1, 0))) mask |= TileGridComponent.Mask.PresentW;
        else mask |= TileGridComponent.Mask.AbsentW;
        if (GetNeighbor(tiles, origin, new Vector2I(1, -1))) mask |= TileGridComponent.Mask.PresentNE;
        else mask |= TileGridComponent.Mask.AbsentNE;
        if (GetNeighbor(tiles, origin, new Vector2I(1, 1))) mask |= TileGridComponent.Mask.PresentSE;
        else mask |= TileGridComponent.Mask.AbsentSE;
        if (GetNeighbor(tiles, origin, new Vector2I(-1, -1))) mask |= TileGridComponent.Mask.PresentNW;
        else mask |= TileGridComponent.Mask.AbsentNW;
        if (GetNeighbor(tiles, origin, new Vector2I(-1, 1))) mask |= TileGridComponent.Mask.PresentSW;
        else mask |= TileGridComponent.Mask.AbsentSW;

        return mask;
    }

    bool GetNeighbor(Dictionary<Vector2I, bool> tiles, Vector2I origin, Vector2I offset) =>
        tiles.GetValueOrDefault(origin + offset, false);
}