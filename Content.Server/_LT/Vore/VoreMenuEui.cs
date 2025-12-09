using System.Linq;
using System.Net.Http.Headers;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Server.Preferences.Managers;
using Content.Shared.Eui;
using Content.Shared._LT;
using Robust.Server.Player;
using Robust.Shared.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
namespace Content.Server._LT;

public sealed class VoreMenuEui : BaseEui
{
    public BellyComponent BCP;
    public ICommonSession Sesh;
    public Belly? CurrentBelly = null;
    [Dependency] private readonly IServerPreferencesManager _prefsManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IServerDbManager _dbManager = default!;
    public VoreSystem vs;
    public VoreComponent vc;


    public VoreMenuEui(BellyComponent bcp, ICommonSession sesh, VoreSystem vs, VoreComponent vc)
    {
        BCP = bcp;
        Sesh =  sesh;
        this.vs = vs;
        this.vc = vc;
    }
    public override VoreMenuEuiState GetNewState()
    {
        List<Belly> guts = new List<Belly>();
        foreach (BellyContentPair bvp in BCP.tums)
        {
            guts.Add(bvp.B);
        }

        Dictionary<string, string> cgspreturnval = new Dictionary<string, string>();
        if (CurrentBelly != null)
        {
            cgspreturnval = vs.LookUpBellyContents(BCP.tums.First(instance => instance.B.Id == CurrentBelly.Id));
        }
        return new VoreMenuEuiState(guts,CurrentBelly, vc.PredPref, vc.PreyPref,cgspreturnval);
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        VoreMenuMessage msgcasted = (VoreMenuMessage)msg;
        switch (msgcasted.action)
        {
            case VoreMenuMessageEnum.Select:
                CurrentBelly = BCP.tums.First(i => i.B.Id == msgcasted.target).B;
                StateDirty();
                break;
            case VoreMenuMessageEnum.Rename:
                CurrentBelly!.Name = msgcasted.arg;
                break;
            case VoreMenuMessageEnum.ChangeDesc:
                CurrentBelly!.InnerDescription = msgcasted.arg;
                break;
            case VoreMenuMessageEnum.SaveToSlot:
                SaveToSlot();
                break;
            case VoreMenuMessageEnum.SelectDM:
                CurrentBelly!.Mode = (BellyDigestMode)Convert.ToByte(msgcasted.arg);
                break;
            case VoreMenuMessageEnum.AddTum:
                AddTum();
                break;
            case VoreMenuMessageEnum.IngestDesc:
                CurrentBelly!.IngestDesc = msgcasted.arg;
                break;
            case VoreMenuMessageEnum.ExpellDesc:
                CurrentBelly!.ExpellDesc = msgcasted.arg;
                break;
            case VoreMenuMessageEnum.DigestPredDesc:
                CurrentBelly!.DigestDescPred = msgcasted.arg;
                break;
            case VoreMenuMessageEnum.DigestPreyDesc:
                CurrentBelly!.DigestDescPrey = msgcasted.arg;
                break;
            case VoreMenuMessageEnum.TogglePredPref:
                vc.PredPref = !vc.PredPref;
                StateDirty();
                break;
            case VoreMenuMessageEnum.TogglePreyPref:
                vc.PreyPref = !vc.PreyPref;
                StateDirty();
                break;
            case VoreMenuMessageEnum.Eject:
                Eject(msgcasted.arg);
                break;
        }

    }

    private void SaveToSlot()
    {
        vs.SaveSlot(Sesh);
    }

    public void AddTum()
    {
        BCP.NewBelly();
        vs.SaveSlot(Sesh);
        StateDirty();
    }

    public void Eject(string input)
    {
        int intcastedstring = Convert.ToInt32(input);
        foreach (BellyContentPair bvp in BCP.tums)
        {
            foreach (var nom in bvp.C.ContainedEntities)
            {
                if (intcastedstring == nom.Id)
                {
                    vs.Eject(nom,bvp.C);
                }
            }
        }
        StateDirty();
    }
}
