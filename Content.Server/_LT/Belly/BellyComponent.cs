using Content.Shared._LT;
using Content.Shared.Damage;
using Content.Shared.Preferences;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;

namespace Content.Server._LT;

[RegisterComponent, AutoGenerateComponentState(true)]
public sealed partial class BellyComponent : Component
{
    [DataField]
    public List<BellyContentPair> tums = new List<BellyContentPair>();

    public void ArrangeGuts(EntityUid uid,HumanoidCharacterProfile profile)
    {
        foreach (Shared._LT.Belly b in profile.Tummies)
        {
            Container c = IoCManager.Resolve<IEntityManager>().System<SharedContainerSystem>().EnsureContainer<Container>(uid, "belly-" + b.Id);
            tums.Add(new BellyContentPair(b,c));
        }
    }

    public void NewBelly()
    {
        if (tums.Count < 20)
        {
            Belly b = new Belly();
            Container c = IoCManager.Resolve<IEntityManager>()
                .System<SharedContainerSystem>()
                .EnsureContainer<Container>(Owner, "belly-" + b.Id);
            tums.Add(new BellyContentPair(b, c));
        }
    }

}
