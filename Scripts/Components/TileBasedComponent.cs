using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using TileSharp.Ecs;

namespace TileSharp.Components;

[GlobalClass]
public partial class TileBasedComponent : ComponentBase
{
    [Export] public Vector2I Position = new(0, 0);
    [Export] public Texture2D TileTexture;
}