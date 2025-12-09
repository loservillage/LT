using Content.Shared._LT;
using Content.Shared.Damage;
using Robust.Shared.Containers;
using Robust.Shared.Serialization;

namespace Content.Server._LT;

public sealed class BellyContentPair
{
    public Belly B;
    public Container C;
    public BellyContentPair(Belly b, Container c)
    {
        B = b;
        C = c;
    }


}
