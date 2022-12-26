using System.Linq;
using Content.Server._Afterlight.Worldgen.Components;
using Content.Server._Citadel.Worldgen;
using Content.Server._Citadel.Worldgen.Components.Debris;
using Content.Server._Citadel.Worldgen.Systems;
using Content.Server.Administration.Logs;
using Content.Server.GameTicking;
using Content.Server.Maps;
using Content.Shared._Afterlight.Worldgen;
using Content.Shared.Database;
using Content.Shared.GameTicking;
using Robust.Server.Maps;
using Robust.Server.Player;
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
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly IAdminLogManager _log = default!;
    [Dependency] private readonly IMapManager _map = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeNetworkEvent<RequestShipSpawnEvent>(OnRequestShipSpawnEvent);
        SubscribeLocalEvent<LoadingMapsEvent>(OnLoadingMaps);
        SubscribeLocalEvent<TickerJoinGameEvent>(OnPlayerJoinGame);
    }

    private void OnPlayerJoinGame(TickerJoinGameEvent ev)
    {
        throw new NotImplementedException();
    }

    private void OnLoadingMaps(LoadingMapsEvent ev)
    {
        ev.Maps.Clear();
        var valid = _prototype.EnumeratePrototypes<GameMapPrototype>().Where(x => x.ValidShip).ToList();

        var slotsLeft = _player.PlayerCount;

        while (slotsLeft > 0)
        {
            var map = _random.Pick(valid);
            var slotsToRemove = Math.Min(map.Stations.Sum(x => x.Value.AvailableJobs.Sum(x => x.Value[0] ?? 0)) / 2, 1);
            slotsLeft -= slotsToRemove;

            ev.Maps.Add(map);
        }
    }

    public void SpawnVessel(string vessel, IPlayerSession? user = null)
    {
        if (!_prototype.TryIndex<GameMapPrototype>(vessel, out var proto))
        {
            return;
        }

        ShipSpawningComponent map;

        {
            var maps = EntityQuery<ShipSpawningComponent>().ToList();
            map = _random.Pick(maps);
        }

        _random.Shuffle(map.FreeCoordinates);

        var safetyBounds = Box2.UnitCentered.Enlarged(48);
        foreach (var coords in map.FreeCoordinates)
        {
            if (_map.FindGridsIntersecting(coords.MapId, safetyBounds.Translated(coords.Position)).Any())
                continue;

            var loadOpts = new MapLoadOptions()
            {
                Offset = coords.Position,
                Rotation = _random.NextAngle(),
                LoadMap = false
            };

            _gameTicker.LoadGameMap(proto, coords.MapId, loadOpts);
            if (user is { } session)
                _log.Add(LogType.ALSpawnVessel, LogImpact.Medium, $"User {session} bought the vessel {proto.ID} ({proto.MapName})");
            return;
        }
    }

    private void OnRequestShipSpawnEvent(RequestShipSpawnEvent msg, EntitySessionEventArgs args)
    {
        SpawnVessel(msg.Vessel, (IPlayerSession)args.SenderSession);
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

            comp.Setup = true;
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
