using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using TileSharp.Systems;

namespace TileSharp.Ecs;

public partial class World : Node
{
    public int Guid { get; init; } = ECS.Instance.LastGuid;

    private readonly Dictionary<Type, List<Entity>> _componentIndex = new();
    public IReadOnlyDictionary<Type, List<Entity>> ComponentIndex => _componentIndex;

    private List<Entity> _entities = new();
    public IReadOnlyList<Entity> Entities => _entities;

    private SubViewport _subViewport;
    public SubViewport Viewport {
        get => _subViewport;
        set => _subViewport ??= value;
    }
    
    public event Action<Type, Entity> OnEntityAddedToIndex;
    public event Action<Type, Entity> OnEntityRemovedFromIndex;
    //public delegate void AddToIndex(string componentType, Entity entity);

    //[Signal]
    //public delegate void RemoveFromIndexEventHandler(string componentType, Entity entity);

    // [Signal]
    // public delegate void RemoveEntityFromIndexEventHandler(Entity entity);
    //public List<Entity> QueryType<T>() where T : Component => _componentIndex.GetValueOrDefault(typeof(T));

    public List<Entity> QueryEntities(IEnumerable<Type> whitelist, IEnumerable<Type> blacklist)
    {
        var result = new HashSet<Entity>();
        var whitelistSet = new HashSet<Type>(whitelist);
        var blacklistSet = new HashSet<Type>(blacklist);

        foreach (var (type, entities) in _componentIndex)
        {
            if (whitelistSet.Contains(type))
                foreach (var entity in entities)
                    result.Add(entity);
            else if (blacklistSet.Contains(type))
                foreach (var entity in entities)
                    result.Remove(entity);
        }

        return result.ToList();
    }

    public List<Entity> QueryEntities(IEnumerable<Type> whitelist)
    {
        var result = new HashSet<Entity>();
        var whitelistSet = new HashSet<Type>(whitelist);

        foreach (var (type, entities) in _componentIndex)
        {
            if (whitelistSet.Contains(type))
                foreach (var entity in entities)
                    result.Add(entity);
        }

        return result.ToList();
    }
    public List<Entity> QueryEntities(Type whitelist)
    {
        var result = new HashSet<Entity>();

        foreach (var (type, entities) in _componentIndex)
        {
            if (type == whitelist)
                foreach (var entity in entities)
                    result.Add(entity);
        }

        return result.ToList();
    }


    /// <summary>
    /// Adds a specific component to the entity index, containing component types and entities with them. This is called automatically when calling AddComponent() on an entity, or adding an entity to the world.
    /// </summary>
    /// <param name="entity">The entity to index.</param>
    /// <param name="component">The component to add.</param>
    public void IndexEntityComponent(Entity entity, ComponentBase component)
    {
        if (!entity.Components.Contains(component)) return;
        var type = component.GetType();
        if (!_componentIndex.ContainsKey(type)) _componentIndex[type] = new List<Entity>();
        OnEntityAddedToIndex?.Invoke(type, entity);
        _componentIndex[type].Add(entity);
    }

    public void UnindexEntityComponent(Entity entity, ComponentBase component)
    {
        //if (!entity.Components.Contains(component)) return;
        var type = component.GetType();
        if (!_componentIndex.ContainsKey(type)) return;
        OnEntityRemovedFromIndex?.Invoke(type, entity);
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
            if (!_componentIndex[type].Contains(entity))
            {
                OnEntityAddedToIndex?.Invoke(type, entity);
                _componentIndex[type].Add(entity);
            }
        }
    }

    /// <summary>
    /// Loops through every entity in the world and calls IndexEntity on them. This is pointless to call in most cases, as adding a component or entity already indexes it.
    /// </summary>
    public void IndexEntities()
    {
        foreach (var entity in _entities) IndexEntity(entity);
    }

    /// <summary>
    /// Removes an entity from the whole index.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    public void UnindexEntity(Entity entity)
    {
        foreach (var (type, list) in _componentIndex)
        {
            OnEntityRemovedFromIndex?.Invoke(type, entity);
            list.Remove(entity);
        }
    }

    /// <summary>
    /// Adds an entity to the world and indexes it, calling IndexEntity().
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The entity which was added.</returns>
    public Entity AddEntity(Entity entity)
    {
        //entity.Name = entity.EntityName + '_' + entity.Guid;
        entity.MyWorld = this;
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
        entity.MyWorld = this;
        _entities.Add(entity);
        return entity;
    }

    /// <summary>
    /// Generates a new entity, gives it a name and adds it to the world.
    /// </summary>
    /// <param name="name">The given name.</param>
    /// <returns>The newly created entity.</returns>
    public Entity AddEntity(string name)
    {
        var entity = new Entity(name);
        entity.MyWorld = this;
        _entities.Add(entity);
        return entity;
    }

    /// <summary>
    /// Unindexes and frees an entity.
    /// </summary>
    /// <param name="entity">The entity to free.</param>
    public void RemoveEntity(Entity entity)
    {
        if (!_entities.Contains(entity)) return;
        _entities.Remove(entity);
        UnindexEntity(entity);
        entity.Free();
    }

    public void AddSystem<T>() where T : SystemBase, new()
    {
        // I put a TO DO here, but I don't remember why... 
        AddChild(new T());
    }


    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {
    }
}