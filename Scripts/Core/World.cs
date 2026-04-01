using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace TileSharp.Ecs;

public partial class World : Node
{
    public int Guid { get; init; } = ECS.Instance.LastGuid;

    private readonly Dictionary<Type, List<Entity>> _componentIndex = new();
    public IReadOnlyDictionary<Type, List<Entity>> ComponentIndex => _componentIndex;

    private List<Entity> _entities = new();
    public IReadOnlyList<Entity> Entities => _entities;
    
    public List<Entity> Query<T>() where T : Type => _componentIndex.GetValueOrDefault(typeof(T));
    
    /// <summary>
    /// Adds a specific component to the entity index, containing component types and entities with them. This is called automatically when calling AddComponent() on an entity, or adding an entity to the world.
    /// </summary>
    /// <param name="entity">The entity to index.</param>
    /// <param name="component">The component to add.</param>
    public void IndexEntityComponent(Entity entity, Component component)
    {
        if (!entity.Components.Contains(component)) return;
        var type = component.GetType();
        if (!_componentIndex.ContainsKey(type)) _componentIndex[type] = new List<Entity>();
        _componentIndex[type].Add(entity);
    }

    public void UnindexEntityComponent(Entity entity, Component component)
    {
        //if (!entity.Components.Contains(component)) return;
        var type = component.GetType();
        if (!_componentIndex.ContainsKey(type)) return;
        _componentIndex[type].Remove(entity);
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
        foreach (var entity in _entities) IndexEntity(entity);
    }

    public void UnindexEntity(Entity entity)
    {
        foreach (var list in _componentIndex.Values) list.Remove(entity);
    }

    /// <summary>
    /// Adds an entity to the world and indexes it, calling IndexEntity().
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The entity which was added.</returns>
    public Entity AddEntity(Entity entity)
    {
        //entity.Name = entity.EntityName + '_' + entity.Guid;
        entity.World = this;
        _entities.Add(entity);
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
        entity.World = this;
        _entities.Add(entity);
        return entity;
    }

    public Entity AddEntity(string name)
    {
        var entity = new Entity(name);
        entity.World = this;
        _entities.Add(entity);
        return entity;
    }

    public void RemoveEntity(Entity entity)
    {
        if (!_entities.Contains(entity)) return;
        _entities.Remove(entity);
        UnindexEntity(entity);
        entity.Free();
    }


    public override void _Ready()
    {
        GD.Print("Hello World!");
        var ent = AddEntity();
        GD.Print(ECS.Instance.GetEntityByGuid(1) == ent);
        ent.AddComponent(GD.Load<HelloComponent>("uid://c2gvq7b7gl3pg"));
        GD.Print("Ok");
    }

    public override void _Process(double delta)
    {
    }
}