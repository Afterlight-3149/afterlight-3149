using Content.Server._Afterlight.Worldgen.Systems;
using Robust.Shared.Map;

namespace Content.Server._Afterlight.Worldgen.Components;

/// <summary>
/// This is used for ship spawning, marking locations safe to warp them in.
/// </summary>
[RegisterComponent, Access(typeof(ShipSpawningSystem))]
public sealed class ShipSpawningComponent : Component
{
    /// <summary>
    /// A list of spaces that should be free for spawning.
    /// </summary>
    [DataField("freeCoordinates")]
    public readonly List<MapCoordinates> FreeCoordinates = new();

    /// <summary>
    /// Working around a limitation with ComponentStartup and when it fires.
    /// </summary>
    [DataField("setup")]
    public bool Setup = false;

    [DataField("loadedSpawnArea", required: true)]
    public int LoadedSpawnArea = 16;
}
