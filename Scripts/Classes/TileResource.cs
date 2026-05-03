using Godot;
using System;
using Godot.Collections;
using TileSharp.Components;

[GlobalClass]
public partial class TileResource : Resource
{
    [Export] public Texture2D TileTexture;

    [Export] public int MaxHealth = 100;
    public int Health;
    // @formatter:off
    [ExportGroup("Autotile Mapping")]
    [Export] public bool UseCustomMapping = false;
    [Export] public Godot.Collections.Array<TileGridComponent.Mask> Mapping;
    
    [ExportGroup("Autotile Whitelist")] 
    [Export] public bool AutotileWithOtherTiles = false;
    [Export] public Array<string> TileTags = [];
    [Export] public Array<string> WhitelistedTags = [];
    [Export] public Array<string> BlacklistedTags = [];
    // @formatter:on
}