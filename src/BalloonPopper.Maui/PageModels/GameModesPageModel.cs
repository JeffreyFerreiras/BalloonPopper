using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BalloonPopper.Maui.PageModels;

public class GameModesPageModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async Task StartClassicModeAsync()
    {
        // Pass game mode parameter and navigate to game
        await Shell.Current.GoToAsync($"//GamePage?mode=classic");
    }

    public async Task StartTimeAttackModeAsync()
    {
        // Pass game mode parameter and navigate to game
        await Shell.Current.GoToAsync($"//GamePage?mode=timeattack");
    }

    public async Task StartEndlessModeAsync()
    {
        // Pass game mode parameter and navigate to game
        await Shell.Current.GoToAsync($"//GamePage?mode=endless");
    }

    public async Task StartPrecisionModeAsync()
    {
        // Pass game mode parameter and navigate to game
        await Shell.Current.GoToAsync($"//GamePage?mode=precision");
    }

    public async Task NavigateBackAsync()
    {
        // Navigate back to main menu
        await Shell.Current.GoToAsync("//MenuPage");
    }
}
