using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;

namespace BalloonPopper.Maui
{
    public partial class AppShell : Shell
    {
        private bool _hasNavigatedToMenu = false;

        public AppShell()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Navigate to MenuPage only once when the Shell first appears
            if (!_hasNavigatedToMenu)
            {
                _hasNavigatedToMenu = true;
                await Shell.Current.GoToAsync("//MenuPage");
            }
        }
    }
}
