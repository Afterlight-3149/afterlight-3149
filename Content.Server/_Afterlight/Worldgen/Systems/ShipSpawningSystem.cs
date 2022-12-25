using System.Linq;
using Content.Server._Afterlight.Worldgen.Components;
using Content.Server._Citadel.Worldgen;
using Content.Server._Citadel.Worldgen.Components.Debris;
using Content.Server._Citadel.Worldgen.Systems;
using Content.Server.Maps;
using Content.Shared._Afterlight.Worldgen;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._Afterlight.Worldgen.Systems;

/// <summary>
/// This handles reserving locations to spawn ships.
/// </summary>
public sealed class ShipSpawningSystem : BaseWorldSystem
{
    [Dependency] private readonly IMapManager _map = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeNetworkEvent<RequestShipSpawnEvent>(OnRequestShipSpawnEvent);
    }

    private void OnRequestShipSpawnEvent(RequestShipSpawnEvent msg, EntitySessionEventArgs args)
    {
        ShipSpawningComponent map;

        {
            var maps = EntityQuery<ShipSpawningComponent>().ToList();
            map = _random.Pick(maps);
        }

        _random.Shuffle(map.FreeCoordinates);

        if (!_prototype.TryIndex<GameMapPrototype>(msg.GameMapPrototype, out var proto))
        {

        }

        var safetyBounds = Box2.UnitCentered.Enlarged(48);
        foreach (var coords in map.FreeCoordinates)
        {
            if (_map.FindGridsIntersecting(coords.MapId, safetyBounds.Translated(coords.Position)).Any())
                continue;


        }
    }

    public override void Update(float frameTime)
    {
        //ew
        foreach (var comp in EntityQuery<ShipSpawningComponent>())
        {
            if (comp.Setup)
                continue;

            for (var i = -comp.LoadedSpawnArea; i < comp.LoadedSpawnArea; i++)
            {
                for (var j = -comp.LoadedSpawnArea; j < comp.LoadedSpawnArea; j++)
                {
                    var cCoords = new Vector2i(i, j);
                    var chunk = GetOrCreateChunk(cCoords, comp.Owner);
                    if (!TryComp<DebrisFeaturePlacerControllerComponent>(chunk, out var debris))
                    {
                        continue;
                    }

                    if (debris.OwnedDebris.Count != 0)
                        continue;
                    comp.FreeCoordinates.Add(new MapCoordinates(WorldGen.ChunkToWorldCoordsCentered(cCoords), Comp<MapComponent>(comp.Owner).WorldMap));
                }
            }
        }
    }
}

/// <summary>
///
/// </summary>
/// <param name="GameMapPrototype"></param>
/// <param name="Location"></param>
/// <param name="CancelledLocal">Whether to cancel spawning for this specific location.</param>
/// <param name="CancelledGlobal">Whether to universally prevent spawning.</param>
public record struct TrySpawnShipEvent(string GameMapPrototype, MapCoordinates Location, bool CancelledLocal = false, bool CancelledGlobal = false);
