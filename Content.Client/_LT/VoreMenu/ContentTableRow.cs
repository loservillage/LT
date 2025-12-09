using Robust.Client.UserInterface.Controls;

namespace Content.Client._LT.VoreMenu;

public sealed class ContentTableRow
{
    public string id;
    public Label La;
    //public Button ExamineBtn;
    public Button EjectBtn;

    public ContentTableRow(KeyValuePair<string, string> kvp)
    {
        id = kvp.Value;
        La = new Label();
        La.Text = kvp.Key;
        //ExamineBtn = new Button();
        EjectBtn = new Button();
        EjectBtn.Text =  "Eject";
        EjectBtn.Name = "Eject;" + kvp.Value;
    }
}
