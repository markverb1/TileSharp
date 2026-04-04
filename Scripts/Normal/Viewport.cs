using System.Linq;
using Godot;
using TileSharp.Components;
using TileSharp.Ecs;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TileSharp;

public partial class Viewport : Control
{
    private SubViewportContainer _viewportContainer;

    private void HandleWorldCreated(World world)
    {
        var viewport = new SubViewport();
        viewport.Name = world.Name;
        _viewportContainer.AddChild(viewport);
        world.Viewport = viewport;
    }

    public override void _Ready()
    {
        // var resource = GD.Load<BasicInfoComponent>("uid://c1ahwhalspod5");
        // resource.ResourcePath = "";
        // var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).IgnoreFields()
        //     .Build();
        // var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
        //     .IgnoreFields().IgnoreUnmatchedProperties().Build();
        //
        // var ser = serializer.Serialize(resource);
        // var deser = deserializer.Deserialize<BasicInfoComponent>(ser);
        //
        // GD.Print(ser);
        // GD.Print(deser);

        _viewportContainer = GetNode<SubViewportContainer>("ViewportContainer");
        ECS.Instance.WorldCreated += HandleWorldCreated;
        ECS.Instance.CreateWorld("World");
    }

    public override void _Process(double delta)
    {
    }
}