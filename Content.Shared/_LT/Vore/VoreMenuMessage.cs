using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._LT;

[Serializable,NetSerializable]
public sealed class VoreMenuMessage : EuiMessageBase
{
    public VoreMenuMessageEnum action;
    public Guid? target;
    public string arg;
    public VoreMenuMessage(VoreMenuMessageEnum action, Guid? target, string arg)
    {
        this.action = action;
        this.target = target;
        this.arg = arg;
    }
}
[Serializable,NetSerializable]
public enum VoreMenuMessageEnum : byte
{
    Select,
    Rename,
    ChangeDesc,
    SaveToSlot,
    SelectDM,
    AddTum,
    IngestDesc,
    ExpellDesc,
    DigestPredDesc,
    DigestPreyDesc,
    TogglePredPref,
    TogglePreyPref,
    Eject,

}
