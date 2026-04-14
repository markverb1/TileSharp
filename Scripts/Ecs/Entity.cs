using System.Collections.Generic;
using System.Linq;
using Godot;

namespace TileSharp.Ecs;

[GlobalClass, Icon("res://Assets/EditorIcons/Entity.svg")]
public partial class Entity : Resource
{
    private readonly List<ComponentBase> _components = new();
    public IReadOnlyList<ComponentBase> Components => _components;
    public int Guid { get; } = ECS.Instance.LastGuid;
    public World MyWorld { get; internal set; }
    public StringName EntityName;
    
    public Entity() => EntityName = Guid.ToString();

    public Entity(string name) => EntityName = name;

    /// <summary>
    /// Creates a new component and adds it to the entity.
    /// </summary>
    /// <typeparam name="T">Type of component.</typeparam>
    /// <returns>Created component.</returns>
    public ComponentBase AddComponent<T>() where T : ComponentBase, new()
    {
        var c = _components.OfType<T>().FirstOrDefault();
        if (c != null) return c;
        var component = new T();
        _components.Add(component);
        MyWorld.IndexEntityComponent(this, component);
        return component;
    }

    /// <summary>
    /// Adds a component. If the component type already exists, this function replaces it.
    /// </summary>
    public ComponentBase AddComponent(ComponentBase component)
    {
        var type = component.GetType();
        var existingIndex = _components.FindIndex(x => x.GetType() == type);
        if (existingIndex != -1)
        {
            _components[existingIndex] = component;
            return component;
        }

        _components.Add(component);
        MyWorld.IndexEntityComponent(this, component);
        return component;
    }

    public void RemoveComponent<T>() where T : ComponentBase
    {
        var component = _components.Find(x => x.GetType() == typeof(T));
        if (component != null)
        {
            MyWorld.UnindexEntityComponent(this, component);
            _components.Remove(component);
            //component.Free();
        }
    }

    public T GetComponent<T>() where T : ComponentBase
    {
        return _components.Find(x => x.GetType() == typeof(T)) as T;
    }
    public bool HasComponent<T>() where T : ComponentBase => GetComponent<T>() != null;
}