using System;
using System.Collections.Generic;
using Godot;
using TileSharp.Ecs;

namespace TileSharp.Systems;

public partial class HelloSystem : SystemBase
{
    protected override List<Type> WhitelistedTypes { get; } = [typeof(HelloComponent)];
    protected override List<Type> BlacklistedTypes { get; } = [];

    protected override void _SystemReady()
    {
        GD.Print("HelloSystem");
    }

    public override void _Process(double delta)
    {
        foreach (var entity in Entities)
        {
            var component = entity.GetComponent<HelloComponent>();
            if (component.IterationCount < component.Iterations){
                component.IterationCount++;
                GD.Print(component.Text);
            }
        }
    }
}
