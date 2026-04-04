using Godot;
using TileSharp.Ecs;

namespace TileSharp.Components;

[GlobalClass]
public partial class TileTransformComponent : ComponentBase
{
    [Export] public Vector2I Position = new(0,0);
    
}
