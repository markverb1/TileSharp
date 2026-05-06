using System;
using Godot;
using System.Linq;

public sealed partial class Util : Node
{
    public static Util Instance { get; private set; }

    public bool HasChildOfType<T>(Node parent) where T : Node
    {
        return parent.GetChildren().OfType<T>().Any();
    }

    public Node FindChildOfType<T>(Node parent) where T : Node
    {
        return parent.GetChildren().OfType<T>().FirstOrDefault();
    }

    public override void _Ready()
    {
        Instance = this;
    }
}