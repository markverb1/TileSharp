using System;
using System.Numerics;
using Godot;

public partial class ViewportHandler : SubViewport
{
    private int _shrink = 1;
    private int Shrink
    {
        get => _shrink;
        set => _shrink = Math.Clamp(value, 1, 5);
    }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Shrink = 1;
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var screenSize = DisplayServer.WindowGetSize();
        // GD.Print(GetViewport() == this);
        // GD.Print("Screensize     : " + screenSize);
        Size2DOverride = new Vector2I(screenSize.X/Shrink, screenSize.Y/Shrink); 
        // GD.Print("Size2d Override: " + Size2DOverride);
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("zoom_in"))
        {
            Shrink += 1;
        }
        else if (@event.IsActionPressed("zoom_out")) Shrink -= 1;
    }
}