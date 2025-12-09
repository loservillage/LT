using Robust.Client.UserInterface.Controls;

namespace Content.Client._LT.VoreMenu;

public sealed class BellyButton
{
    public Guid reference;
    public Button button;

    public BellyButton(Guid reference, Button button)
    {
        this.reference = reference;
        this.button = button;
    }
}
