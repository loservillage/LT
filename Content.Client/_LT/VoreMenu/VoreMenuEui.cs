using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared._LT;

namespace Content.Client._LT;

public sealed class VoreMenuEui : BaseEui
{
    private VoreMenu.VoreMenu Window;
    public VoreMenuEui()
    {
        Window = new VoreMenu.VoreMenu(this);
    }
    public override void HandleState(EuiStateBase state)
    {
        Window.HandleState((VoreMenuEuiState)state);
    }
    public override void Opened()
    {
        base.Opened();

        Window.OpenCentered();
    }
    public override void Closed()
    {
        base.Closed();

        Window.Close();
        Window.Dispose();
    }

    public void SelectBelly(Guid id)
    {
        SendMessage(new VoreMenuMessage(VoreMenuMessageEnum.Select,id,""));
    }

    public void EditName(string newname)
    {
        SendMessage(new VoreMenuMessage(VoreMenuMessageEnum.Rename,null,newname));
    }

    public void EditDescription(string newdesc)
    {
        SendMessage(new VoreMenuMessage(VoreMenuMessageEnum.ChangeDesc,null,newdesc));
    }

    public void SaveToSlot()
    {
        SendMessage(new VoreMenuMessage(VoreMenuMessageEnum.SaveToSlot,null,""));
    }

    public void SelectDM(string notbyte)
    {
        SendMessage(new VoreMenuMessage(VoreMenuMessageEnum.SelectDM,null,notbyte));
    }

    public void NewGut()
    {
        SendMessage(new VoreMenuMessage(VoreMenuMessageEnum.AddTum,null,""));
    }

    public void EditIngestDesc(string newdesc)
    {
        SendMessage(new VoreMenuMessage(VoreMenuMessageEnum.IngestDesc,null,newdesc));
    }
    public void EditExpellDesc(string newdesc)
    {
        SendMessage(new VoreMenuMessage(VoreMenuMessageEnum.ExpellDesc,null,newdesc));
    }
    public void EditDPredDesc(string newdesc)
    {
        SendMessage(new VoreMenuMessage(VoreMenuMessageEnum.DigestPredDesc,null,newdesc));
    }
    public void EditDPreyDesc(string newdesc)
    {
        SendMessage(new VoreMenuMessage(VoreMenuMessageEnum.DigestPreyDesc,null,newdesc));
    }

    public void TogglePredPref()
    {
        SendMessage(new VoreMenuMessage(VoreMenuMessageEnum.TogglePredPref,null,""));
    }

    public void TogglePreyPref()
    {
        SendMessage(new  VoreMenuMessage(VoreMenuMessageEnum.TogglePreyPref,null,""));
    }

    public void Eject(string euid)
    {
        SendMessage(new VoreMenuMessage(VoreMenuMessageEnum.Eject,null,euid));
    }
}
