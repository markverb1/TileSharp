using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class TileResource : Resource
{
    [Export] public Texture2D TileTexture;
    [Export] public string TileName = "None"; // Name is added as a tag
    [Export] public int MaxHealth = 100;

    public int Health;

    // @formatter:off
    // [ExportGroup("Autotile Mapping")]
    // [Export] public bool UseCustomMapping = false;
    // [Export] public Array<TileGridComponent.Mask> Mapping;
    
    [ExportGroup("Autotile Whitelist")]
    [Export] public bool AutotileWithAllTiles;
    [Export] public Array<string> TileTags = [];
    private HashSet<string> _tileTagsSet;
    public HashSet<string> TileTagsSet => _tileTagsSet ??= [..TileTags, TileName];
    [Export] public Array<string> WhitelistedTags = [];
    private HashSet<string> _whitelistedSet;
    public HashSet<string> WhitelistedSet => _whitelistedSet ??= [..WhitelistedTags];
    [Export] public Array<string> BlacklistedTags = [];
    private HashSet<string> _blacklistedSet;
    public HashSet<string> BlacklistedSet => _blacklistedSet ??= [..BlacklistedTags];
    // @formatter:on

    public bool CanAutotileWith(TileResource other)
    {
        return AutotileWithAllTiles ||
               other.TileTexture == TileTexture ||
               (!other.TileTagsSet.Overlaps(BlacklistedTags)
                && other.TileTagsSet.Overlaps(WhitelistedTags)) ||
               (!TileTagsSet.Overlaps(other.BlacklistedTags)
                && TileTagsSet.Overlaps(other.WhitelistedTags));
    }
}