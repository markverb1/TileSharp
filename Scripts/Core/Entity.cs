using System.Collections.Generic;
using System.Linq;
using Godot;

namespace TileSharp.Ecs;

[GlobalClass, Icon("res://Assets/EditorIcons/Entity.svg")]
public partial class Entity : Node
{
    /// <summary>
    /// This entity's global unique identifier. Gotten from the ECS Autoload when initialized. Cannot be set.
    /// </summary>
    public int Guid { get; init; } = ECS.Instance.LastGuid;

    public StringName EntityName;

    public Entity() => EntityName = "Entity";
    public Entity(string name) => EntityName = name;
    public Entity(StringName name) => EntityName = name;

    private readonly List<Component> _components = new();
    public IReadOnlyList<Component> Components => _components;
    
    /// <summary>
    /// Creates a new component and adds it to the entity.
    /// </summary>
    /// <typeparam name="T">Type of component.</typeparam>
    /// <returns>Created component.</returns>
    public Component AddComponent<T>() where T : Component, new()
    {
        var c = _components.OfType<T>().FirstOrDefault();
        if (c != null) return c;
        var component = new T();
        _components.Add(component);
        return component;
    }

    /// <summary>
    /// Adds a component. If the component type already exists, this function replaces it.
    /// </summary>
    /// <param name="componentClass">The component node.</param>
    /// <returns>The newly added component.</returns>
    public Component AddComponent(Component component)
    {
        var type = component.GetType();
        var existingIndex = _components.FindIndex(x => x.GetType() == type);
        if (existingIndex != -1)
        {
            _components[existingIndex] = component;
            return component;
        }
        _components.Add(component);
        GetWorld().IndexEntity(this, component);
        return component;
    }
    
    public void RemoveComponent<T>() where T : Component
    {
        var component = _components.Find(x => x.GetType() == typeof(T));
        component?.Free();
    }

    public Component GetComponent<T>() where T : Component
    {
        return _components.Find(x => x.GetType() == typeof(T));
    }

    /// <summary>
    /// Check if this entity has a specific component.
    /// </summary>
    /// <typeparam name="T">A component type.</typeparam>
    /// <returns>bool</returns>
    public bool HasComponent<T>() where T : Component
    {
        return GetComponent<T>() != null;
    }

    /// <summary>
    /// Gets the World this entity belongs to.
    /// </summary>
    /// <returns>The world it belongs to.</returns>
    public World GetWorld() => GetParent<Node>().GetParent<World>();

    public override void _ExitTree()
    {
        GetWorld().UnindexEntity(this);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}