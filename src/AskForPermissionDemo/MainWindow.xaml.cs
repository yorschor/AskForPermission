using System.ComponentModel;
using System.Windows;
using AskForPermission;

namespace AskForPermissionDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private string _adminActionResult = "";
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public MainWindow()
    {
        InitializeComponent();
    }

    public string AdminStatus => PermissionPrompt.CheckPermission().ToString();

    public string AdminActionResult
    {
        get => _adminActionResult;
        private set
        {
            _adminActionResult = value;
            OnPropertyChanged(nameof(AdminActionResult));
        }
    }

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
}