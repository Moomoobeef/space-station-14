using Robust.Shared.Timing;
using Content.Shared.EntityEffects.Effects.StatusEffects;
using System.Diagnostics.Tracing;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;

namespace Content.Shared.Addiction;

/// <summary>
/// NicotineAddictionSystem is responsible for triggering effects from a player's addiction,
/// and also reacting to the player's consumption.
/// </summary>
/// <remarks>
/// This can be easily repurposed to use for other reagents.
/// Atleast, I *hope* it's easy, I'm trying to design it to be easy.
/// </remarks>
public sealed class NicotineAddictionSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    //[Dependency] private readonly FixedPoint2 _fixedPoint2 = default!;

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        // Events sent in from the addiction effect. This effect is generalized for *all* addictive chemicals.
        SubscribeLocalEvent<NicotineAddictionComponent, AddictionEvent>(OnAddiction);
    }

    // runs when the player metabolises nicotine
    private void OnAddiction(Entity<NicotineAddictionComponent> ent, ref AddictionEvent args)
    {
        if (args.Reagent == null)
            return; // I have no idea what circumstances would cause this to be null,
                    // but if I don't handle it the IDE is angy with me.

        if (args.Reagent.ID != ent.Comp.Reagent)
            return; // make sure that this is actually nicotine. Because the addiction effect is generic,
                    // *all* addictive chemicals use it, not just nicotine.

        if (ent.Comp.WithdrawalAmount - args.Amount < 0)
            ent.Comp.WithdrawalAmount = 0;
        else
        {
            ent.Comp.WithdrawalAmount = ent.Comp.WithdrawalAmount - args.Amount;
        }
    }

    // So basically, update is responsible for 2 main things
        // - increasing withdrawalAmount over time
        // - and triggering symptoms
    // That second part is interesting, and it's also the main thing you may need to change
    // if you want to adapt this system for other reagents, as symptoms are hard coded.
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;

        // create an enumerator for all nicotine addictions
        var query = EntityQueryEnumerator<NicotineAddictionComponent>();

        // This loop is responsible for raising the withdrawalAmount variable over time
        while (query.MoveNext(out var uid, out var comp))
        {
            // Skip this entity if it should not be updated yet.
            if (comp.NextWithdrawal > curTime)
                continue;

            // increase withdrawal amount
            comp.WithdrawalAmount = comp.WithdrawalAmount + 0.02; // how much to raise it by

            // pick the next interval time
            comp.NextWithdrawal += comp.UpdateInterval;
        }

        // down here is going to be symptoms stuff


    }

    private void OnMapInit(Entity<NicotineAddictionComponent> ent, ref MapInitEvent args)
    {
        // We have to set the first update time after the entity is spawned.
        // Without this it will start at time 0 and update every single frame until it catches up to the server time.
        // It is important that this is done in MapInitEvent, not ComponentInit.
        ent.Comp.NextWithdrawal = _timing.CurTime + ent.Comp.UpdateInterval;
    }
}
