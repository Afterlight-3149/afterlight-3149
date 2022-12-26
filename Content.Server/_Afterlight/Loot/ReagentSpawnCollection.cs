using System.Linq;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Random;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._Afterlight.Loot;

/// <summary>
///     Dictates a list of items that can be spawned.
/// </summary>
[Serializable]
[DataDefinition]
public struct ReagentSpawnEntry
{
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("id", customTypeSerializer: typeof(PrototypeIdSerializer<ReagentPrototype>))]
    public string? PrototypeId = null;

    /// <summary>
    ///     The probability that an item will spawn. Takes decimal form so 0.05 is 5%, 0.50 is 50% etc.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("prob")] public float SpawnProbability = 1;

    /// <summary>
    ///     orGroup signifies to pick between entities designated with an ID.
    ///     <example>
    ///         <para>
    ///             To define an orGroup in a StorageFill component you
    ///             need to add it to the entities you want to choose between and
    ///             add a prob field. In this example there is a 50% chance the storage
    ///             spawns with Y or Z.
    ///         </para>
    ///         <code>
    /// - type: StorageFill
    ///   contents:
    ///     - name: X
    ///     - name: Y
    ///       prob: 0.50
    ///       orGroup: YOrZ
    ///     - name: Z
    ///       orGroup: YOrZ
    /// </code>
    ///     </example>
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("orGroup")] public string? GroupId = null;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("amount")] public int Amount = 1;

    /// <summary>
    ///     How many of this can be spawned, in total.
    ///     If this is lesser or equal to <see cref="Amount"/>, it will spawn <see cref="Amount"/> exactly.
    ///     Otherwise, it chooses a random value between <see cref="Amount"/> and <see cref="MaxAmount"/> on spawn.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("maxAmount")] public int MaxAmount = 1;

    public ReagentSpawnEntry() { }
}

/// <summary>
/// A faster version of EntitySpawnCollection that requires caching to work.
/// </summary>
public sealed class ReagentSpawnCollection
{
    private sealed class OrGroup
    {
        [ViewVariables]
        public List<ReagentSpawnEntry> Entries { get; set; } = new();
        [ViewVariables]
        public float CumulativeProbability { get; set; }
    }

    [ViewVariables]
    private readonly Dictionary<string, OrGroup> _orGroups = new();

    public ReagentSpawnCollection(IEnumerable<ReagentSpawnEntry> entries)
    {
        // collect groups together, create singular items that pass probability
        foreach (var entry in entries)
        {
            if (!_orGroups.TryGetValue(entry.GroupId ?? string.Empty, out var orGroup))
            {
                orGroup = new();
                _orGroups.Add(entry.GroupId ?? string.Empty, orGroup);
            }

            orGroup.Entries.Add(entry);
            orGroup.CumulativeProbability += entry.SpawnProbability;
        }
    }

    /// <summary>
    ///     Using a collection of entity spawn entries, picks a random list of entity prototypes to spawn from that collection.
    /// </summary>
    /// <remarks>
    ///     This does not spawn the entities. The caller is responsible for doing so, since it may want to do something
    ///     special to those entities (offset them, insert them into storage, etc)
    /// </remarks>
    /// <param name="random">Resolve param.</param>
    /// <param name="toFill">The solution to fill, if any.</param>
    /// <returns>A list of entity prototypes that should be spawned.</returns>
    /// <remarks>This is primarily useful if you're calling it many times over, as it lets you reuse the list repeatedly.</remarks>
    public void GetSpawns(IRobustRandom random, ref Solution toFill)
    {
        // handle orgroup spawns
        foreach (var spawnValue in _orGroups.Values)
        {
            //HACK: This doesn't seem to work without this if there's only a single orgroup entry. Not sure how to fix the original math properly, but it works in every other case.
            if (spawnValue.Entries.Count == 1)
            {
                var entry = spawnValue.Entries.First();
                var amount = entry.Amount;

                if (entry.MaxAmount > amount)
                    amount = random.Next(amount, entry.MaxAmount);

                if (entry.PrototypeId is null)
                    continue;

                toFill.AddReagent(entry.PrototypeId, amount);

                continue;
            }

            // For each group use the added cumulative probability to roll a double in that range
            var diceRoll = random.NextDouble() * spawnValue.CumulativeProbability;
            // Add the entry's spawn probability to this value, if equals or lower, spawn item, otherwise continue to next item.
            var cumulative = 0.0;
            foreach (var entry in spawnValue.Entries)
            {
                cumulative += entry.SpawnProbability;
                if (diceRoll > cumulative)
                    continue;
                // Dice roll succeeded, add item and break loop

                var amount = entry.Amount;

                if (entry.MaxAmount > amount)
                    amount = random.Next(amount, entry.MaxAmount);

                if (entry.PrototypeId is null)
                    continue;

                toFill.AddReagent(entry.PrototypeId, amount);

                break;
            }
        }
    }
}
