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
    private TileSet _tileSet = new();
    private Dictionary<Texture, int> _textureToAtlasId = new();

    public int GetAtlasTextureId(Texture2D texture)
    {
        if (texture is null) return -1;
        if (_textureToAtlasId.TryGetValue(texture ?? new Texture2D(), out var atlasId)) return atlasId;
        var source = new TileSetAtlasSource();
        source.TextureRegionSize = _tileSet.TileSize;
        source.Texture = texture;
        for (int i = 0; i < 14; i++) source.CreateTile(new Vector2I(i % 7, i / 7));
        int id = _tileSet.AddSource(source);
        _textureToAtlasId[texture] = id;
        return id;
    }

    protected override void _SystemReady()
    {
        TilesParent = new Node2D();
        TilesParent.Name = "TilesParent";
        World.Viewport.AddChild(TilesParent);

        var tgComponent = new TileGridComponent();
        //var idx = 0;
        _tileSet.TileSize =  new Vector2I(32, 32);
        //_tileSet.TileSize = tgComponent.TileSize;
        for (int idx = 0; idx < tgComponent.DefaultNeighborMapping.Length; idx++)
        {
            var tileLayer = new TileMapLayer();
            _layers.Add(tileLayer);
            tileLayer.Name = idx.ToString();
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
                var example = GD.Load<TileResource>("res://Assets/Resources/TileResource/example.tres");
                var wall = GD.Load<TileResource>("res://Assets/Resources/TileResource/wall.tres");

                tgComponent.Tiles.Add(new Vector2I(1, 1), wall);
                tgComponent.Tiles.Add(new Vector2I(1, 2), wall);
                tgComponent.Tiles.Add(new Vector2I(1, 3), wall);
                tgComponent.Tiles.Add(new Vector2I(2, 1), example);
                //tgComponent.Tiles.Add(new Vector2I(2, 2), true);
                tgComponent.Tiles.Add(new Vector2I(2, 3), wall);
                tgComponent.Tiles.Add(new Vector2I(3, 1), example);
                tgComponent.Tiles.Add(new Vector2I(3, 2), wall);
                tgComponent.Tiles.Add(new Vector2I(3, 3), example);
                foreach (var layer in _layers) layer.TileSet = _tileSet;
                tgComponent.FirstTimeProcessed = true;
            }

            foreach (var tile in tgComponent.Tiles)
            {
                if (tile.Value is null) continue;
                var bitmask = GetBitMask(tgComponent.Tiles, tile.Key);
                int layerIdx = 0;
                foreach (var layer in _layers)
                {
                    if (bitmask.HasFlag(tgComponent.DefaultNeighborMapping[layerIdx]))
                        layer.SetCell(tile.Key, GetAtlasTextureId(tile.Value.TileTexture),
                            new Vector2I(layerIdx % 7, layerIdx / 7));
                    layerIdx++;
                }
            }
        }
    }

    TileGridComponent.Mask GetBitMask(Dictionary<Vector2I, TileResource> tiles, Vector2I origin)
    {
        TileGridComponent.Mask mask = 0;

        if (GetNeighbor(tiles, origin, new Vector2I(0, -1)) is not null) mask |= TileGridComponent.Mask.PresentN;
        else mask |= TileGridComponent.Mask.AbsentN;
        if (GetNeighbor(tiles, origin, new Vector2I(0, 1)) is not null) mask |= TileGridComponent.Mask.PresentS;
        else mask |= TileGridComponent.Mask.AbsentS;
        if (GetNeighbor(tiles, origin, new Vector2I(1, 0)) is not null) mask |= TileGridComponent.Mask.PresentE;
        else mask |= TileGridComponent.Mask.AbsentE;
        if (GetNeighbor(tiles, origin, new Vector2I(-1, 0)) is not null) mask |= TileGridComponent.Mask.PresentW;
        else mask |= TileGridComponent.Mask.AbsentW;
        if (GetNeighbor(tiles, origin, new Vector2I(1, -1)) is not null) mask |= TileGridComponent.Mask.PresentNE;
        else mask |= TileGridComponent.Mask.AbsentNE;
        if (GetNeighbor(tiles, origin, new Vector2I(1, 1)) is not null) mask |= TileGridComponent.Mask.PresentSE;
        else mask |= TileGridComponent.Mask.AbsentSE;
        if (GetNeighbor(tiles, origin, new Vector2I(-1, -1)) is not null) mask |= TileGridComponent.Mask.PresentNW;
        else mask |= TileGridComponent.Mask.AbsentNW;
        if (GetNeighbor(tiles, origin, new Vector2I(-1, 1)) is not null) mask |= TileGridComponent.Mask.PresentSW;
        else mask |= TileGridComponent.Mask.AbsentSW;

        return mask;
    }

    TileResource GetNeighbor(Dictionary<Vector2I, TileResource> tiles, Vector2I origin, Vector2I offset) =>
        tiles.GetValueOrDefault(origin + offset, null);
}