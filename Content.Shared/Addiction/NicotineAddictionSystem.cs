using Robust.Shared.Timing;
using Content.Shared.EntityEffects.Effects.StatusEffects;
using System.Diagnostics.Tracing;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using System.Linq;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

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
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

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
            comp.WithdrawalAmount += 0.02; // how much to raise it by

            // pick the next interval time
            comp.NextWithdrawal += comp.UpdateInterval;

            ///////////////////////////////////////
            // ## Down here is symptoms stuff ## //

            // Get the prototype that contains data about the addiction.
            // See /Resources/Prototypes/Addiction/addictions.yml
            var addictionPrototype = _prototypeManager.Index<AddictionPrototype>(comp.Reagent.Id);

            // Get the info about our symptoms.
            var symptoms = addictionPrototype.Symptoms;

            //Console.WriteLine(string.Join(",", symptoms));
        }

        // down here is going to be symptoms stuff
        var rand = new System.Random();
        var random = rand.NextDouble(); // random number, used for random chance of triggering a symptom

        // 6/5/25
        // Doing this in psuedo-code just to get the concept straightened out before I actually
        // start trying to implement it in C#

        // 1. Random probability to trigger a withdrawal symptom, weighted by withdrawal amount
        //    When withdrawal amount = 0, no symptoms. as it gets higher more symptoms will happen
        //    (I should probably cap withdrawal amount at like 20 or something (20 being the nicotine
        //    content of two cigarettes.)
        // 2. If random symptom chance passes, then we need to pick a symptom from a set of avaliable.
        //    There will be multiple ones, with different severities. We don't want the player to start
        //    taking damage when they just smoked a minute ago, so I am thinking there will be a
        //    prototype which contains cut-off values for all the symptoms, basically below this number
        //    this symptom will not trigger.
        //   2a. This means I need to set up a way of bringing this data into the system, likely through
        //       the component.
        //   2b. Somehow I need to relate the names of symptoms in the prototype to the actual code, which
        //       is probably not too hard, but everything is easier said than done in C#.
        // 3. Once we know which symptoms we *can* use, we need to pick one. This will probably just be
        //    a random integer between 0 and whatever the number of the last one we *can* use is, followed
        //    by a switch-case.
        //
        // Note: I had originally thought about giving individual symptoms different probabilities, however
        //       that would be a pain in the ass and this is complex enough, so just the approach of gating
        //       symptoms based on withdrawalAmount is fine.

        // 6/7/25
        // my first idea on how to implement symptom cutoffs was dumb. I re-thought it and also how I am going
        // to implement symptoms altogether.

        // NEW PLAN:
        // different types of symptoms will be coded not in here, where they have to be duplicated and tweaked
        // for every separate addiction, but instead in a separate file. This file will just call classes from
        // there to run them.
        // Instead of storing a "cutoffs" array in the prototype which just contains an ordered list of numbers
        // corresponding to the symptoms *as they are literally ordered in the code* (ew)
        // I am going to have a string with the name of the symptom, followed by the cutoff value for that
        // symptom. This is a MUCH BETTER implementation, and it means that symptom code will be the same
        // between different AddictionSystems. If you want to add new symptoms, just add them to the SymptomSystem
        // and add them to the prototype.

        // Yes this is a more complicated idea, but it is *way* better, and tbf I don't think the previous
        // idea would have survived review anyway because its so shitty and unmaintainable.
    }

    private void OnMapInit(Entity<NicotineAddictionComponent> ent, ref MapInitEvent args)
    {
        // We have to set the first update time after the entity is spawned.
        // Without this it will start at time 0 and update every single frame until it catches up to the server time.
        // It is important that this is done in MapInitEvent, not ComponentInit.
        ent.Comp.NextWithdrawal = _timing.CurTime + ent.Comp.UpdateInterval;
    }
}
