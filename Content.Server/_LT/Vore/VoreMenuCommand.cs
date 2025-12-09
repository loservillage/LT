/*using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared._LT;
using Robust.Shared.Console;

namespace Content.Server.LT;
[AnyCommand]
public sealed class VoreMenuCommand : LocalizedCommands
{
    [Dependency] private readonly EuiManager _eui = default!;
    public override string Command => "voremenu";
    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not {} player)
        {
            shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
            return;
        }

        var ui = new VoreMenuEui(shell.Player);
        _eui.OpenEui(ui, shell.Player);
        ui.StateDirty();
    }
}
*/
