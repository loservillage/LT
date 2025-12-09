using System.Linq;
using Content.Server._Corvax.Respawn;
using Content.Server.Atmos.Components;
using Content.Server.Carrying;
using Content.Server.Chat.Managers;
using Content.Server.Database;
using Content.Server.DoAfter;
using Content.Server.EUI;
using Content.Server.Forensics;
using Content.Server.Mind;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Preferences.Managers;
using Content.Server.Temperature.Components;
using Content.Shared._LT;
using Content.Shared.Administration;
using Content.Shared.Chat;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Forensics.Components;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.PowerCell.Components;
using Content.Shared.Preferences;
using Content.Shared.Verbs;
using Discord;
using Robust.Shared.Console;
using Robust.Shared.Containers;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Serialization;

namespace Content.Server._LT;


public sealed class VoreSystem : VoreSystemShared
{
    [Dependency] private readonly EuiManager _eui = default!;
    [Dependency] private readonly IConsoleHost _consoleHost = default!;
    [Dependency] private readonly IServerPreferencesManager _prefsManager = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly MindSystem _serverMindSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popups = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly CarryingSystem _carrying = default!;
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly HungerSystem _hunger = default!;
    [Dependency] private readonly BatterySystem _battery = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly InventorySystem _inventorySystem = default!;
    [Dependency] private readonly RespawnSystem _respawnSystem = default!;

    public float Ticker = 0;
    public override void Initialize()
    {
        base.Initialize();
        _consoleHost.RegisterCommand("voremenu",
            "Opens the vore menu.",
            "Opens the vore menu",
            VoreMenuCommand);

        SubscribeLocalEvent<VoreComponent, GetVerbsEvent<InnateVerb>>(AddVerbs);
        SubscribeLocalEvent<VoreComponent, VoreDoAfterEvent>(OnDoAfter);
        SubscribeLocalEvent<MobStateComponent,ComponentStartup>(InitMob);
    }

    private void InitMob(EntityUid uid, MobStateComponent component, ComponentStartup args)
    {
    }

    private void AddVerbs(EntityUid uid, VoreComponent component, GetVerbsEvent<InnateVerb> args)
    {
        if ((
                !args.CanInteract
                ||
                args.User == args.Target
                ||
                !TryComp<VoreComponent>(args.User, out VoreComponent? voruser)
                ||
                !voruser.PredCanditate
                ||
                !TryComp<BellyComponent>(args.User, out BellyComponent? belly)
                ||
                belly.tums.Count == 0
                ||
                !TryComp<VoreComponent>(args.Target, out VoreComponent? vortarget)
                ||
                !vortarget.PreyCanditate
                ||
                !FetchPredPref(args.User, voruser)
                ||
                !FetchPreyPref(args.Target, vortarget)
            ) // <-- Thanks floof for the yandev if hell
           )
        {
            return;
        }
        else
        {
            foreach (var bkp in belly.tums)
            {

                InnateVerb verbDevour = new()
                {
                    Act = () => TryDevour(args.User, args.Target, component,bkp),
                    Text = bkp.B.Name,
                    Category = VerbCategory.Devour,
                };

                args.Verbs.Add(verbDevour);
            }

        }


    }

    public void TryDevour(EntityUid uid, EntityUid target, VoreComponent? component, BellyContentPair bkp)
    {
        string preyname;
        if (!Resolve(uid, ref component))
            return;

        _entityManager.TryGetComponent<MetaDataComponent>(uid, out MetaDataComponent? predmeta);
        _entityManager.TryGetComponent<MetaDataComponent>(target, out MetaDataComponent? preymeta);
        _popups.PopupEntity(predmeta!.EntityName + " is trying to " + bkp.B.IngestDesc + " " + preymeta!.EntityName + " into their " + bkp.B.Name, uid,  PopupType.MediumCaution);


        /*if (!TryComp<PhysicsComponent>(uid, out var predPhysics)
            || !TryComp<PhysicsComponent>(target, out var preyPhysics))
            return;*/

        var length = TimeSpan.FromSeconds(component.DoAfterDelay);
                                          /* * _contests.MassContest(preyPhysics, predPhysics, false, 4f) // Big things are harder to fit in small things
                                          * _contests.StaminaContest(isInsertion?target:uid, isInsertion?uid:target) // The person doing the action having higher stamina makes it easier
                                          * (_standingState.IsDown(isInsertion?uid:target) ? 0.5f : 1)); // If the person having the action done to them is on the ground it's easier*/

        _doAfterSystem.TryStartDoAfter(new DoAfterArgs(EntityManager, uid, length, new VoreDoAfterEvent(bkp.B.Id), uid, target: target)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            RequireCanInteract = true
        });
    }

    private void OnDoAfter(EntityUid uid, VoreComponent component, VoreDoAfterEvent args)
    {
        if (args.Target is null
            || args.Cancelled)
            return;

        TryComp<BellyComponent>(uid, out BellyComponent? belly);


        Devour(uid, args.Target.Value, belly!.tums.First(instance => instance.B.Id == args.BellyID));
    }

    public void Devour(EntityUid uid, EntityUid target, BellyContentPair bkp)
    {
        EnsureComp<PressureImmunityComponent>(target);
        if (TryComp<TemperatureComponent>(target, out var temp))
        {
            temp.AtmosTemperatureTransferEfficiency = 0;
        }
        _carrying.DropCarried(uid, target);
        _carrying.DropCarried(target, uid);
        _containerSystem.Insert(target, bkp.C);
        _entityManager.TryGetComponent<MetaDataComponent>(uid, out MetaDataComponent? predmeta);
        _entityManager.TryGetComponent<MetaDataComponent>(target, out MetaDataComponent? preymeta);
        _popups.PopupEntity(predmeta!.EntityName  + " "+ bkp.B.IngestDesc + " "+ preymeta!.EntityName + " into their " + bkp.B.Name, uid,  PopupType.MediumCaution);
    }



    public bool FetchPreyPref(Entity<MindContainerComponent?> q, VoreComponent vc)
    {
        if (!Resolve(q, ref q.Comp)
            || _serverMindSystem.GetMind(q, q) is not { } mind)
        {
            return true; // NPCs as well as player characters without a mind consent to everything <-- Why without mind??? Okay??? I have no idea how to better check for this so we are using this dumbass thing
        }
        if (!TryComp<MindComponent>(mind, out var mindComponent)
            || mindComponent.UserId is not { } userId)
        {
            // Not sure if this is ever reached? MindComponent seems to always have UserId.
            Log.Warning("HasConsent No UserId or missing MindComponent");
            return false; // For entities that have a mind but with no user attached, consent to nothing.
        }

        return vc.PreyPref;

    }
    public bool FetchPredPref(Entity<MindContainerComponent?> q, VoreComponent vc)
    {
        if (!Resolve(q, ref q.Comp)
            || _serverMindSystem.GetMind(q, q) is not { } mind)
        {
            return true; // NPCs as well as player characters without a mind consent to everything <-- Why without mind??? Okay??? I have no idea how to better check for this so we are using this dumbass thing
        }
        if (!TryComp<MindComponent>(mind, out var mindComponent)
            || mindComponent.UserId is not { } userId)
        {
            // Not sure if this is ever reached? MindComponent seems to always have UserId.
            Log.Warning("HasConsent No UserId or missing MindComponent");
            return false; // For entities that have a mind but with no user attached, consent to nothing.
        }

        return vc.PredPref;

    }

    [AnyCommand]
    public async void VoreMenuCommand(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not {} player)
        {
            shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
            return;
        }

        if (shell.Player.AttachedEntity == null)
        {
            shell.WriteError(Loc.GetString("shell-must-be-attached-to-entity"));
            return;
        }

        BellyComponent? bc = Comp<BellyComponent>((EntityUid)shell.Player.AttachedEntity);
        VoreComponent? vc = Comp<VoreComponent>((EntityUid)shell.Player.AttachedEntity);
        var ui = new VoreMenuEui(bc,shell.Player,this,vc);
        _eui.OpenEui(ui, shell.Player);
        ui.StateDirty();
    }

    public async void SaveSlot(ICommonSession sesh)
    {
        _prefsManager.TryGetCachedPreferences(sesh.UserId,out var prefs);
        var profile = (HumanoidCharacterProfile)prefs!.SelectedCharacter;
        profile.Tummies.Clear();
        BellyComponent? bc = Comp<BellyComponent>((EntityUid)sesh.AttachedEntity!);
        foreach (var b in bc.tums)
        {
            profile.Tummies.Add(b.B);
        }
        _prefsManager.SetProfile(sesh.UserId, prefs.SelectedCharacterIndex, profile);
    }

    public Dictionary<string, string> LookUpBellyContents(BellyContentPair bcp)
    {
        Dictionary<string, string> returnval = new Dictionary<string, string>();
        foreach (EntityUid CE in bcp.C.ContainedEntities)
        {
            _entityManager.TryGetComponent<MetaDataComponent>(CE, out MetaDataComponent? CEMD);
            returnval.Add(CEMD!.EntityName, CE.Id.ToString());
        }

        return returnval;
    }

    public void Eject(EntityUid uid, Container bcon)
    {
        _containerSystem.Remove(uid,bcon);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        Ticker += frameTime;
        if (Ticker > 1)
        {
            Ticker -= 1;
        }
        else
        {
            return;
        }
        EntityQueryEnumerator<BellyComponent> TummiesToProcess = EntityQueryEnumerator<BellyComponent>();
        while (TummiesToProcess.MoveNext(out EntityUid euid, out BellyComponent? bc))
        {
            foreach (BellyContentPair bcp in bc.tums)
            {
                switch (bcp.B.Mode)
                {
                    case BellyDigestMode.Digest:
                        Digest(bcp, euid);
                        break;
                    default:
                        break;

                }
            }
        }

    }

    public void Digest(BellyContentPair bcp, EntityUid owner)
    {
        CauseCaustic(bcp, owner);
    }

    /// <summary>
    /// Don't forget.
    /// </summary>
    public void CauseCaustic(BellyContentPair bcp, EntityUid owner)
    {
        DamageSpecifier damage = new();
        damage.DamageDict.Add("Caustic", bcp.B.DigestDamageCaustic) ;
        DamageAll(bcp, damage,owner);

    }

    public void DamageAll(BellyContentPair bcp, DamageSpecifier dam, EntityUid owner)
    {
        foreach (EntityUid euid in bcp.C.ContainedEntities)
        {

            if (_mobState.IsDead(euid))
            {
                if (_playerManager.TryGetSessionByEntity(owner, out ICommonSession? sessionpred)
                    || sessionpred is not null)
                {
                    _chatManager.ChatMessageToOne(
                        ChatChannel.Emotes,
                        bcp.B.DigestDescPred,
                        bcp.B.DigestDescPred,
                        EntityUid.Invalid,
                        false,
                        sessionpred.Channel);
                }

                if (_playerManager.TryGetSessionByEntity(euid, out ICommonSession? sessionprey)
                    || sessionprey is not null)
                {
                    _chatManager.ChatMessageToOne(
                        ChatChannel.Emotes,
                        bcp.B.DigestDescPrey,
                        bcp.B.DigestDescPrey,
                        EntityUid.Invalid,
                        false,
                        sessionprey.Channel);
                }

                if (TryComp<InventoryComponent>(euid, out var inventoryComponent) &&
                    _inventorySystem.TryGetSlots(euid, out var slots))
                {
                    foreach (var slot in slots)
                    {
                        if (_inventorySystem.TryGetSlotEntity(euid, slot.Name, out EntityUid? item, inventoryComponent))
                        {
                            if (item != null)
                            {
                                _containerSystem.Insert(item.Value, bcp.C);
                            }

                        }
                    }
                }
                _damageable.TryChangeDamage(euid, dam, true, false);
                if (TryComp<HungerComponent>(owner, out var hunger))
                    _hunger.ModifyHunger(owner, 100, hunger);

                if (TryComp<BatteryComponent>(owner, out var internalbattery))
                    _battery.SetCharge(owner, internalbattery.CurrentCharge + 200, internalbattery);

                if (TryComp<PowerCellSlotComponent>(owner, out var batterySlot)
                    && _containerSystem.TryGetContainer(owner, batterySlot.CellSlotId, out var container)
                    && container.ContainedEntities.Count > 0)
                {
                    var battery = container.ContainedEntities.First();
                    if (TryComp<BatteryComponent>(battery, out var batterycomp))
                        _battery.SetCharge(battery, batterycomp.CurrentCharge + 200, batterycomp);
                }
                if (TryComp<BellyComponent>(euid, out var preytums))
                    foreach (var preytum in preytums.tums)
                    {
                        foreach (var item in preytum.C.ContainedEntities)
                        {
                            _containerSystem.Insert(item, bcp.C);
                        }
                    }

                /*if (!_entityManager.TryGetComponent(euid, out ActorComponent? actorc))
                {
                    var respawnData = _respawnSystem.GetRespawnData(actorc.PlayerSession.UserId);
                    _respawnSystem.SetRespawnTime(actorc.PlayerSession.UserId, ref respawnData, TimeSpan.Zero);
                }*/
                //
                QueueDel(euid);
            }
            else
            {
                _damageable.TryChangeDamage(euid, dam, true, false);
                if (TryComp<HungerComponent>(owner, out var hunger))
                    _hunger.ModifyHunger(owner, 10, hunger);

                if (TryComp<BatteryComponent>(owner, out var internalbattery))
                    _battery.SetCharge(owner, internalbattery.CurrentCharge + 20, internalbattery);

                if (TryComp<PowerCellSlotComponent>(owner, out var batterySlot)
                    && _containerSystem.TryGetContainer(owner, batterySlot.CellSlotId, out var container)
                    && container.ContainedEntities.Count > 0)
                {
                    var battery = container.ContainedEntities.First();
                    if (TryComp<BatteryComponent>(battery, out var batterycomp))
                        _battery.SetCharge(battery, batterycomp.CurrentCharge + 20, batterycomp);
                }
            }
        }
    }
}


