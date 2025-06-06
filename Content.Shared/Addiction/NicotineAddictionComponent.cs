using Robust.Shared.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameStates;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Addiction;

/// <summary>
/// Used to make a character addicted to a chemical.
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class NicotineAddictionComponent : Component
{
    /// <summary>
    /// Which reagent will the player be addicted to?
    /// </summary>
    [DataField]
    public ProtoId<ReagentPrototype> Reagent = "Nicotine";

    /// <summary>
    /// WithdrawalAmount will increase over time, and affect the severity of symptoms.
    /// Consuming the addicted reagent will lower it.
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    public FixedPoint2 WithdrawalAmount;

    /// <summary>
    /// The server time at which the withdrawal amount will go up.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoNetworkedField, AutoPausedField]
    public TimeSpan NextWithdrawal = TimeSpan.Zero;

    /// <summary>
    /// How often to update.
    /// In this case every two seconds.
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    public TimeSpan UpdateInterval = TimeSpan.FromSeconds(2);
}
