namespace Content.Server._Afterlight.Worldgen.Components;

/// <summary>
/// This is used for chunks that interfere with radio transmission.
/// </summary>
[RegisterComponent]
public sealed class RadioInterferingChunkComponent : Component
{
    [DataField("allowHighPower")]
    public bool AllowHighPower = true;
}
