using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace TileSharp.Ecs;

public partial class World : Node
{
    public int Guid { get; init; } = ECS.Instance.LastGuid;

    private readonly Dictionary<Type, HashSet<Entity>> _componentIndex = new();
    public IReadOnlyDictionary<Type, HashSet<Entity>> ComponentIndex => _componentIndex;

    private HashSet<Entity> _entities = new();
    public IReadOnlySet<Entity> Entities => _entities;

    private SubViewport _subViewport;

    public SubViewport Viewport
    {
        get => _subViewport;
        set => _subViewport ??= value;
    }

    private Dictionary<Type, List<Delegate>> _broadcastSubscribers = new();
    private Dictionary<Type, List<(Type component, Delegate callback)>> _directedSubscribers = new();


    public event Action<Type, Entity> OnEntityAddedToIndex;
    public event Action<Type, Entity> OnEntityRemovedFromIndex;


    public HashSet<Entity> QueryEntities(IEnumerable<Type> whitelist, IEnumerable<Type> blacklist,
        bool allRequired = true)
    {
        HashSet<Entity> result = QueryEntities(whitelist, allRequired);
        var blacklistSet = new HashSet<Type>(blacklist);

        foreach (var type in blacklistSet)
        {
            if (!_componentIndex.TryGetValue(type, out var entities)) continue;
            result ??= new HashSet<Entity>();
            result.ExceptWith(entities);
            if (result.Count == 0) return [];
        }

        return result;
    }

    public HashSet<Entity> QueryEntities(IEnumerable<Type> whitelist, bool allRequired = true)
    {
        var whitelistSet = new HashSet<Type>(whitelist);
        HashSet<Entity> result = null;

        if (!allRequired)
            foreach (var type in whitelistSet)
            {
                if (!_componentIndex.TryGetValue(type, out var entities)) continue;
                result ??= new HashSet<Entity>();
                result.UnionWith(entities);
            }
        else
            foreach (var type in whitelistSet)
            {
                if (!_componentIndex.TryGetValue(type, out var entities)) continue;
                result ??= [..entities];
                result.IntersectWith(entities);
                if (result.Count == 0) return [];
            }


        return result ?? [];
    }

    public HashSet<Entity> QueryEntities(Type whitelist)
    {
        var result = new HashSet<Entity>();
        foreach (var (type, entities) in _componentIndex)
            if (type == whitelist)
                result.UnionWith(entities);
        return result;
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
        if (!_componentIndex.ContainsKey(type)) _componentIndex[type] = new HashSet<Entity>();
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
            if (!_componentIndex.ContainsKey(type)) _componentIndex[type] = new HashSet<Entity>();
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