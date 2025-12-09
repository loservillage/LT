using Robust.Shared.Serialization;
using Content.Shared.Eui;

namespace Content.Shared._LT;

[Serializable, NetSerializable]
public sealed class VoreMenuEuiState : EuiStateBase
{
    public List<Belly> Tums;
    public Belly? CurrentTum;
    public Dictionary<string, string> Contentsgsp;
    public bool predpref;
    public bool preypref;
    public VoreMenuEuiState(List<Belly> tums, Belly? ct, bool predpref, bool preypref, Dictionary<string, string> Contentsgsp)
    {
        Tums = tums;
        CurrentTum = ct;
        this.predpref = predpref;
        this.preypref = preypref;
        this.Contentsgsp = Contentsgsp;
    }
}

