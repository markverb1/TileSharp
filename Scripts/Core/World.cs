using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace TileSharp.Ecs;

public partial class World : Node
{
    public int Guid { get; init; } = ECS.Instance.LastGuid;

    private readonly Dictionary<Type, List<Entity>> _componentIndex = new();

    private Node _entitiesNode;
    private Node _systemsNode;

    /// <summary>
    /// Adds a specific component to the entity index, containing component types and entities with them. This is called automatically when calling AddComponent() on an entity, or adding an entity to the world.
    /// </summary>
    /// <param name="entity">The entity to index.</param>
    /// <param name="component">The component to add.</param>
    public void IndexEntity(Entity entity, Component component)
    {
        if (!entity.Components.Contains(component)) return;
        var type = component.GetType();
        if (!_componentIndex.ContainsKey(type)) _componentIndex[type] = new List<Entity>();
        _componentIndex[type].Add(entity);
    }

    /// <summary>
    /// Adds components to the entity index, containing component types and entities with them. This is called automatically when calling AddComponent() on an entity, or adding an entity to the world.
    /// </summary>
    /// <param name="entity">The entity to index.</param>
    public void IndexEntity(Entity entity)
    {
        foreach (var component in entity.Components)
        {
            var type = component.GetType();
            if (!_componentIndex.ContainsKey(type)) _componentIndex[type] = new List<Entity>();
            if (!_componentIndex[type].Contains(entity)) _componentIndex[type].Add(entity);
        }
    }

    /// <summary>
    /// Loops through every entity in the world and calls IndexEntity on them. This is pointless to call in most cases, as adding a component or entity already indexes it.
    /// </summary>
    public void IndexEntities()
    {
        foreach (var entity in _entitiesNode.GetChildren())
            if (entity is Entity e)
                IndexEntity(e);
    }

    public void UnindexEntity(Entity entity)
    {
        foreach (var list in _componentIndex.Values)
            list.Remove(entity);
    }

    /// <summary>
    /// Adds an entity to the world and indexes it, calling IndexEntity().
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The entity which was added.</returns>
    public Entity AddEntity(Entity entity)
    {
        entity.Name = entity.EntityName + '_' + entity.Guid;
        _entitiesNode.AddChild(entity);
        IndexEntity(entity);
        return entity;
    }

    /// <summary>
    /// Generates a new entity and adds it to the world.
    /// </summary>
    /// <returns>The newly created entity.</returns>
    public Entity AddEntity()
    {
        var entity = new Entity();
        _entitiesNode.AddChild(entity);
        entity.Name = "Entity_" + entity.Guid;
        return entity;
    }

    public Entity AddEntity(string name)
    {
        var entity = new Entity(name);
        _entitiesNode.AddChild(entity);
        entity.Name = name + '_' + entity.Guid;
        return entity;
    }

    public void RemoveEntity(Entity entity) => entity.QueueFree();


    public override void _Ready()
    {
        _entitiesNode = new Node();
        _entitiesNode.Name = "Entities";
        AddChild(_entitiesNode);

        _systemsNode = new Node();
        _systemsNode.Name = "Systems";
        AddChild(_systemsNode);

        GD.Print("Hello World!");
        var ent = AddEntity();
        GD.Print(ECS.Instance.GetECSNodeByGuid(1) == ent);
        ent.AddComponent<Component>();
    }

    public override void _Process(double delta)
    {
    }
}