using Godot;
using TileSharp.Ecs;
using YamlDotNet.Serialization;

namespace TileSharp.Components;

[GlobalClass]
public partial class BasicInfoComponent : ComponentBase
{
    [Export] public string FullName { get; set; } = "Nothing";
    [Export] public string ShortName{ get; set; } = "None";
    [Export] public string Description { get; set; }= "";
}
