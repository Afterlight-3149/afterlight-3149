using Robust.Shared.Serialization;

namespace Content.Shared._Afterlight.Worldgen;

[Serializable, NetSerializable]
public sealed class RequestShipSpawnEvent : EntityEventArgs
{
    public string Vessel;

    public RequestShipSpawnEvent(string vessel)
    {
        Vessel = vessel;
    }
}

[Serializable, NetSerializable]
public record UpdateSpawnEligibilityEvent(bool Eligible);
