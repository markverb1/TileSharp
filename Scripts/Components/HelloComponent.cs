using Godot;

[GlobalClass]
public partial class HelloComponent : Component
{
    [Export] private string Text = "Hello World!";
}
