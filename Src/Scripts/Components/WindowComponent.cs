using Godot;
using System;
using TileSharp.Ecs;

[GlobalClass]
public partial class WindowComponent : ComponentBase
{
    [Export] public string Title;
    [Export] public Vector2I Size;
    [ExportGroup("Flags")] [Export] public bool Visible = true;
    [Export] public bool Resizeable = true;
    [Export] public bool AlwaysOnTop;
    [Export] public bool Focusable = true;
    [Export] public bool Minimizeable = true;
    [Export] public bool Closeable = true;
    [ExportGroup("Limits")] [Export] public Vector2I MinSize = new(100, 100); 
    [Export] public Vector2I MaxSize = new(16384, 16384);
    [Export] public bool KeepTitleVisible = true;
    
}