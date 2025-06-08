using Content.Shared.Damage;
using Content.Shared.Physics;
using Content.Shared.Weapons.Reflect;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Addiction;

[Prototype]
public sealed partial class AddictionPrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Contains a list of cut-off values for all the addictions symptoms.
    /// </summary>
    [DataField("symptoms", required: true)]
    public required Array Symptoms;
}
