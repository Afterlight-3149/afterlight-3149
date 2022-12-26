using Content.Server._Afterlight.Worldgen.Components;
using Content.Server._Citadel.Worldgen.Systems;
using Content.Server.Radio.EntitySystems;

namespace Content.Server._Afterlight.Worldgen.Systems;

/// <summary>
/// This handles worldgen chunk driven radio interference.
/// </summary>
public sealed class RadioInterferingChunkSystem : BaseWorldSystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<TryHeadsetTransmitEvent>(OnTryHeadsetTransmit);
    }

    private void OnTryHeadsetTransmit(ref TryHeadsetTransmitEvent ev)
    {
        var xform = Transform(ev.Transmitter);
        var chunk = GetOrCreateChunk(GetChunkCoords(ev.Transmitter, xform), xform.MapUid!.Value);
        if (TryComp<RadioInterferingChunkComponent>(chunk, out var interference))
            ev.Cancelled = interference.AllowHighPower ? !ev.HighPower : true;

    }
}
