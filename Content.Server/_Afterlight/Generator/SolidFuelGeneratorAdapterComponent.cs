namespace Content.Server._Afterlight.Generator;

/// <summary>
/// This is used for allowing you to insert fuel into gens.
/// </summary>
[RegisterComponent, Access(typeof(GeneratorSystem))]
public sealed class SolidFuelGeneratorAdapterComponent : Component
{
    /// <summary>
    /// The material to accept as fuel.
    /// </summary>
    [DataField("fuelMaterial"), ViewVariables(VVAccess.ReadWrite)]
    public string FuelMaterial = "Plasma";

    /// <summary>
    /// How much fuel that material should count for.
    /// </summary>
    [DataField("multiplier"), ViewVariables(VVAccess.ReadWrite)]
    public float Multiplier = 1.0f;
}
