using Robust.Shared.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameStates;

namespace Content.Shared.Addiction;

/// <summary>
/// Used to make a character addicted to a chemical.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class NicotineAddictionComponent : Component
{
    /// <summary>
    /// Which reagent will the player be addicted to?
    /// </summary>
    [DataField] // TODO: This should probably be a table or something so someone can have multiple addictions.
    public ProtoId<ReagentPrototype> Reagent = "Nicotine";  // But at the moment I'm just trying to do a simple implementation, and allowing multiple addictions will require rethinking all of this.

    public float withdrawalAmount; // This number will grow over time, and the severity of symptoms will depend on it. Consuming the addicted reagent will lower the value.

}