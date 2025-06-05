using Content.Shared.Chemistry.Reagent;
using Content.Shared.StatusEffect;
using Robust.Shared.Prototypes;
using Content.Shared.FixedPoint;


namespace Content.Shared.EntityEffects.Effects.StatusEffects;

/// <summary>
///     
/// </summary>
/// <remarks>
///     
/// </remarks>
public sealed partial class AddictionEffect : EntityEffect
{
    

    public override void Effect(EntityEffectBaseArgs args)
    {
        if (args is not EntityEffectReagentArgs reagentArgs) 
            return;
        
        var evt = new AddictionEvent{
            Amount = reagentArgs.Quantity,
            Reagent = reagentArgs.Reagent
        };
        args.EntityManager.EventBus.RaiseLocalEvent(args.TargetEntity, ref evt);
    }

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) => null;
}

[ByRefEvent]
public record struct AddictionEvent
{
    public FixedPoint2 Amount;
    public ReagentPrototype? Reagent; //No idea what would happen if this is null
}