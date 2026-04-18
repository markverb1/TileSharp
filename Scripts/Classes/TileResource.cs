using Godot;
using System;
using TileSharp.Components;

[GlobalClass]
public partial class TileResource : Resource
{
    [Export] public Texture TileTexture;

    [Export] public Godot.Collections.Array<TileGridComponent.Mask> Mapping;
}