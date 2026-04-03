using System;
using System.Linq;
using Godot;

namespace TileSharp.Ecs;

[Icon("res://Assets/EditorIcons/Ecs.svg")]
public sealed partial class ECS : Node
{
    public static ECS Instance { get; private set; }

    private int _lastGuid = 0;

    public int LastGuid
    {
        get => _lastGuid++;
        private set => _lastGuid = value;
    }

    /// <summary>
    /// Creates a world and adds it to the tree.
    /// </summary>
    /// <param name="name">Name of the world.</param>
    /// <returns>The created world</returns>
    public World CreateWorld(string name)
    {
        var world = new World();
        world.Name = name + "_" + world.Guid;
        AddChild(world);
        return world;
    }

    /// <summary>
    /// Gets any ECS-related node by its GUID, whether it be a World, Entity or System.
    /// </summary>
    /// <param name="guid">GUID of the node</param>
    /// <returns>World, Entity or System</returns>
    // ReSharper disable once InconsistentNaming
    public Entity GetEntityByGuid(int guid)
    {
        foreach (World world in GetChildren().OfType<World>())
        { //if (world.Guid == guid) return world;
            foreach (var entity in world.Entities)
            {
                if (entity.Guid == guid) return entity;
            }
        }
        return null;
    }
    public Node GetECSNodeByGuid(int guid)
    {
        foreach (World world in GetChildren().OfType<World>())
        { 
            if (world.Guid == guid) return world;
        }
        return null;
    }

    public override void _Ready()
    {
        Instance = this;

        CreateWorld("World");
    }
}