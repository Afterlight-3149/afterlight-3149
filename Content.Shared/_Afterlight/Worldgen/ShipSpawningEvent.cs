using Robust.Shared.Serialization;

namespace Content.Shared._Afterlight.Worldgen;

[Serializable, NetSerializable]
public record RequestShipSpawnEvent(string GameMapPrototype);

[Serializable, NetSerializable]
public record UpdateSpawnEligibilityEvent(bool Eligible);
