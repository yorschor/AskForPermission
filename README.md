# Ask for permission

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
