using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Verbs;
using Robust.Shared.Utility;
using Content.Shared.EntityEffects.Effects.StatusEffects;

namespace Content.Shared.Addiction;

/// <summary>
/// 
/// </summary>
public sealed class NicotineAddictionSystem : EntitySystem
{
    [Dependency] private readonly ExamineSystemShared _examine = default!;

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NicotineAddictionComponent, AddictionEvent>(OnAddiction);
    }

    private void OnAddiction(Entity<NicotineAddictionComponent> ent, ref AddictionEvent args)
    {
        var x = 1;
    }

    
}
