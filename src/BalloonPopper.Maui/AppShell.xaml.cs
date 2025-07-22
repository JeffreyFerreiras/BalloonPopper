using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;

namespace BalloonPopper.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Set the default startup page to MenuPage
            Shell.Current.GoToAsync("//MenuPage");
        }
    }
}
