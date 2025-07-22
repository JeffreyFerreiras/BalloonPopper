using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BalloonPopper.Maui.PageModels;

public class MenuPageModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async Task StartGameAsync()
    {
        // Navigate to the main game page
        await Shell.Current.GoToAsync("//GamePage");
    }

    public async Task NavigateToGameModesAsync()
    {
        // Navigate to game modes selection
        await Shell.Current.GoToAsync("//GameModesPage");
    }

    public async Task NavigateToSettingsAsync()
    {
        // Navigate to settings page
        await Shell.Current.GoToAsync("//SettingsPage");
    }

    public async Task ShowAboutAsync()
    {
        // Show about dialog or navigate to about page
        if (Application.Current?.Windows.Count > 0)
        {
            await Application
                .Current.Windows[0]
                .Page!.DisplayAlert(
                    "About Balloon Popper",
                    "🎈 Balloon Popper v1.0.0\n\nA fun and engaging balloon popping game built with .NET MAUI!\n\nPop balloons, score points, and compete for high scores!\n\n© 2025 Balloon Popper Team",
                    "OK"
                );
        }
    }
}
