using Godot;
using System;
using TileSharp.Components;

[GlobalClass]
public partial class TileResource : Resource
{
    [Export] public Texture TileTexture;
    [Export] public bool UseCustomMapping = false;
    [Export] public Godot.Collections.Array<TileGridComponent.Mask> Mapping;
    [Export] public int MaxHealth = 100;
    public int Health;
}