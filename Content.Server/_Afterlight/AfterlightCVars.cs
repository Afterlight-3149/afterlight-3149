using Robust.Shared.Configuration;

namespace Content.Server._Afterlight;

[CVarDefs]
public sealed class AfterlightCVars
{
    /// <summary>
    /// Whether or not world generation is enabled.
    /// </summary>
    public static readonly CVarDef<bool> ShipSpawningEnabled =
        CVarDef.Create("afterlight.shipspawning.enabled", true, CVar.SERVERONLY);

}
