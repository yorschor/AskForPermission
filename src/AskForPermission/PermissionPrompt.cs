using System.Diagnostics;
using System.Security.Principal;

namespace AskForPermission;

public static class PermissionPrompt
{
    /// <summary>
    /// Checks whether the current user has administrator privileges.
    /// </summary>
    /// <returns>True if the current user is an administrator; otherwise, false.</returns>
    public static bool CheckPermission() => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    /// <summary>
    /// Requests administrator privileges for the current application. If the user declines, executes the specified callback.
    /// </summary>
    /// <param name="onDeclineCallBack">The action to execute if the user declines administrator privileges.</param>
    public static void Ask(Action onDeclineCallBack)
    {
        try
        {
            if (CheckPermission()) return;
            Process.Start(new ProcessStartInfo { FileName = Environment.ProcessPath, UseShellExecute = true, Verb = "runas" });
            Environment.Exit(0);
        }
        // User selects "no" from UAC Dialog
        catch
        {
            onDeclineCallBack.Invoke();
        }
    }

}