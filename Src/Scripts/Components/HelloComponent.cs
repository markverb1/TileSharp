using Godot;
using TileSharp.Ecs;

namespace TileSharp.Components;

[GlobalClass]
public partial class HelloComponent : ComponentBase
{
    [Export] public string Text = "Hello world!";
    [Export] public int Iterations = 1;
    public int IterationCount = 0;
}