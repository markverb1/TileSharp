using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace TileSharp.Ecs;

[GlobalClass]
public abstract partial class SystemBase : Node
{
    public World World { get; private set; }

    protected HashSet<Entity> Entities = [];

    protected bool AllWhitelistedRequired = true;
    protected abstract HashSet<Type> WhitelistedTypes { get; }
    protected abstract HashSet<Type> BlacklistedTypes { get; }

    protected void HandleAddToIndex(Type type, Entity entity)
    {
        if (BlacklistedTypes.Contains(type)) return; // We don't care 
    
        var qualifies = AllWhitelistedRequired
            ? WhitelistedTypes.All(t => entity.Components.Any(c => c.GetType() == t))
            : WhitelistedTypes.Any(t => entity.Components.Any(c => c.GetType() == t));
    
        if (qualifies) Entities.Add(entity);
    }

    protected void HandleRemoveFromIndex(Type type, Entity entity)
    {
        if (!WhitelistedTypes.Contains(type) && !BlacklistedTypes.Contains(type)) return;

        var stillQualifies = AllWhitelistedRequired
            ? WhitelistedTypes.All(t => entity.Components.Any(c => c.GetType() == t))
            : WhitelistedTypes.Any(t => entity.Components.Any(c => c.GetType() == t));
    
        var noBlacklisted = !BlacklistedTypes.Any(t => entity.Components.Any(c => c.GetType() == t));

        if (!stillQualifies || !noBlacklisted)
            Entities.Remove(entity);
    }

    public sealed override void _Ready()
    {
        World = GetParent<World>();
        Entities = World.QueryEntities(WhitelistedTypes, BlacklistedTypes, AllWhitelistedRequired);
        World.OnEntityAddedToIndex += HandleAddToIndex;
        World.OnEntityRemovedFromIndex += HandleRemoveFromIndex;
        _SystemReady();
    }

    protected virtual void _SystemReady()
    {
    }

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