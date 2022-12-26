using Content.Shared._Afterlight.Worldgen;

namespace Content.Client._Afterlight.Spawning;

public sealed class ShipSpawningSystem : EntitySystem
{
    public bool Eligible = false;

    public event Action? EligibilityUpdate;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeNetworkEvent<UpdateSpawnEligibilityEvent>(OnUpdate);
    }

    private void OnUpdate(UpdateSpawnEligibilityEvent ev)
    {
        Eligible = ev.Eligible;
        EligibilityUpdate?.Invoke();
    }
}
