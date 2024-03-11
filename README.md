# Ask for permission
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/yorschor/AskForPermission/continuous.yml?branch=trunk&style=flat&label=Trunk)
![NuGet](https://img.shields.io/nuget/v/AskForPermission)
![NuGet Downloads](https://img.shields.io/nuget/dt/AskForPermission?style=flat)
![GitHub License](https://img.shields.io/github/license/yorschor/AskForPermission?style=flat)

## 📚 About 
A small utility class that allows for checking for admin right in windows as well as restarting the current application as an admin via a UAC prompt

## ✍ Usage Example
````csharp
private void ActionThatNeedsAdminRights(object sender, RoutedEventArgs e)
{
    if (!PermissionPrompt.CheckPermission())
    {
        //Ask for UAC and provide a callback for when the prompt exits without success. (e.g. User select no)
        PermissionPrompt.Ask(() => AdminActionResult = "Failure. Admin Prompt says no!");
    }
    else
    {
        //Implementation of your action that needs to run as a admin
        AdminActionResult = "Success!";
    }
}
````
