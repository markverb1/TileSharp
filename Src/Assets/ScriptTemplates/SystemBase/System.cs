using System;
using System.Collections.Generic;
using _BINDINGS_NAMESPACE_;
using TileSharp.Ecs;

namespace TileSharp.Systems;

[GlobalClass]
public partial class _CLASS_ : _BASE_
{
    protected override List<Type> WhitelistedTypes { get; } = [];
    protected override List<Type> BlacklistedTypes { get; } = [];

    protected override void _SystemReady()
    {
    }

    public override void _Process(double delta)
    {
        foreach (var entity in Entities)
        {
        }
    }
}
