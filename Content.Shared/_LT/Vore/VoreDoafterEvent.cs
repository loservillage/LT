using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._LT;

[Serializable, NetSerializable]
public sealed partial class VoreDoAfterEvent : DoAfterEvent
{
    public Guid BellyID;

    public VoreDoAfterEvent(Guid BellyID)
    {
        this.BellyID = BellyID;
    }

    public override DoAfterEvent Clone() => this;

}
