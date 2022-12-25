using System.Diagnostics;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Client._Afterlight;

[Prototype("gameMap"), PublicAPI]
[DebuggerDisplay("GameMapPrototype [{ID} - {MapName}]")]
public sealed partial class GameMapPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// Name of the map to use in generic messages, like the map vote.
    /// </summary>
    [DataField("mapName", required: true)]
    public string MapName { get; } = default!;

    [DataField("description")]
    public string Description { get; } = string.Empty;

    [DataField("validShip")]
    public bool ValidShip = false;
}
