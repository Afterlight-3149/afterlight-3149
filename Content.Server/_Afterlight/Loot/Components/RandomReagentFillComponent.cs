namespace Content.Server._Afterlight.Loot.Components;

/// <summary>
/// This is used for filling a solution container with random contents.
/// </summary>
[RegisterComponent]
public sealed class RandomReagentFillComponent : Component
{
    [DataField("targetSolution", required: true)]
    public string TargetSolution = string.Empty;

    [DataField("entries", required: true)]
    private List<ReagentSpawnEntry> _entries = default!;


    private ReagentSpawnCollection? _cache;

    /// <summary>
    /// The reagent collection.
    /// </summary>
    public ReagentSpawnCollection CachedReagentSpawn
    {
        get
        {
            _cache ??= new ReagentSpawnCollection(_entries);
            return _cache;
        }
    }
}
