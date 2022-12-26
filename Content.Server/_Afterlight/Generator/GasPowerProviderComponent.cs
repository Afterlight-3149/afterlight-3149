using Content.Server.Atmos;

namespace Content.Server._Afterlight.Generator;

/// <summary>
/// This is used for providing gas power to machinery.
/// </summary>
[RegisterComponent]
public sealed class GasPowerProviderComponent : Component
{
    /// <summary>
    /// Past this temperature we assume we're in reaction mass mode and not magic mode.
    /// </summary>
    [DataField("maxTemperature")] public float MaxTemperature = 1000.0f;

    /// <summary>
    /// The amount of plasma consumed for operation in magic mode.
    /// </summary>
    [DataField("plasmaMolesConsumedSec")] public float PlasmaMolesConsumedSec = 1.55975875833f / 4;
    /// <summary>
    /// The amount of kPA "consumed" for operation in pressure mode.
    /// </summary>
    [DataField("pressureConsumedSec")] public float PressureConsumedSec = 100f;
    /// <summary>
    /// Whether the consumed gas should then be ejected directly into the atmosphere.
    /// </summary>
    [DataField("offVentGas")] public bool OffVentGas = false;
    [ViewVariables]
    public TimeSpan LastProcess { get; set; } = TimeSpan.Zero;



    public bool Powered = true;


}
