using System;
using System.Collections.Generic;
using Godot;

namespace TileSharp.Ecs;

[GlobalClass]
public abstract partial class SystemBase : Node
{
    public World World { get; private set; }

    protected List<Entity> Entities = [];

    protected abstract List<Type> WhitelistedTypes { get; }
    protected abstract List<Type> BlacklistedTypes { get; }

    private void HandleAddToIndex(Type type, Entity entity)
    {
        if (WhitelistedTypes.Contains(type) && !BlacklistedTypes.Contains(type))
            Entities.Add(entity);
    }

    private void HandleRemoveFromIndex(Type type, Entity entity)
    {
        if (WhitelistedTypes.Contains(type) && !BlacklistedTypes.Contains(type) && Entities.Contains(entity))
            Entities.Remove(entity);
    }

    public sealed override void _Ready()
    {
        World = GetParent<World>();
        Entities = World.QueryEntities(typeof(Components.HelloComponent));
        World.OnEntityAddedToIndex += HandleAddToIndex;
        World.OnEntityRemovedFromIndex += HandleRemoveFromIndex;
        _SystemReady();
    }

    protected virtual void _SystemReady() {}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // foreach (var entity in _entities)
        // {
        //     var component = entity.GetComponent<HelloComponent>();
        //     if (++component.IterationCount < component.Iterations) GD.Print(component.Text);
        // }
    }
}