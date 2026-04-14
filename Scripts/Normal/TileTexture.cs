using Godot;
using System;

public partial class TileTexture : Resource
{
    [Export] public Texture MainTexture;
    [ExportCategory("Terrain")] 
    [Export] public bool IsTerrain;
    // ReSharper disable InconsistentNaming
    [Export] public Texture Unconnected;
    [Export] public Texture N;
    [Export] public Texture NE;
    [Export] public Texture E;
    [Export] public Texture SE;
    [Export] public Texture S;
    [Export] public Texture SW;
    [Export] public Texture W;
    [Export] public Texture NW;
    // ReSharper restore InconsistentNaming

}