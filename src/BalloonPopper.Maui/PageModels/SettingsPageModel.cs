using System.ComponentModel;
using System.Runtime.CompilerServices;
using BalloonPopper.Services.Abstractions;

namespace BalloonPopper.Maui.PageModels;

public class SettingsPageModel : INotifyPropertyChanged
{
    private readonly IAdService? _adService;

    private bool _soundEffectsEnabled = true;
    private bool _backgroundMusicEnabled = true;
    private double _masterVolume = 80.0;
    private string _selectedDifficulty = "Medium";
    private bool _hapticFeedbackEnabled = true;
    private bool _showFpsCounter = false;
    private bool _isPremium = false;

    public SettingsPageModel(IAdService? adService = null)
    {
        _adService = adService;
        LoadSettings();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool SoundEffectsEnabled
    {
        get => _soundEffectsEnabled;
        set
        {
            if (_soundEffectsEnabled != value)
            {
                _soundEffectsEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public bool BackgroundMusicEnabled
    {
        get => _backgroundMusicEnabled;
        set
        {
            if (_backgroundMusicEnabled != value)
            {
                _backgroundMusicEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public double MasterVolume
    {
        get => _masterVolume;
        set
        {
            if (Math.Abs(_masterVolume - value) > 0.01)
            {
                _masterVolume = value;
                OnPropertyChanged();
            }
        }
    }

    public string SelectedDifficulty
    {
        get => _selectedDifficulty;
        set
        {
            if (_selectedDifficulty != value)
            {
                _selectedDifficulty = value;
                OnPropertyChanged();
            }
        }
    }

    public bool HapticFeedbackEnabled
    {
        get => _hapticFeedbackEnabled;
        set
        {
            if (_hapticFeedbackEnabled != value)
            {
                _hapticFeedbackEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public bool ShowFpsCounter
    {
        get => _showFpsCounter;
        set
        {
            if (_showFpsCounter != value)
            {
                _showFpsCounter = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsPremium
    {
        get => _isPremium;
        set
        {
            if (_isPremium != value)
            {
                _isPremium = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PremiumButtonText));
            }
        }
    }

    public string PremiumButtonText => IsPremium ? "✅ Premium" : "💎 Buy Premium";

    public async Task HandlePremiumPurchaseAsync()
    {
        if (IsPremium)
        {
            // Already premium
            if (Application.Current?.Windows.Count > 0)
            {
                await Application.Current.Windows[0].Page!.DisplayAlert(
                    "Premium Features", 
                    "You already have premium! Thanks for your support! 🎉", 
                    "Awesome!");
            }
            return;
        }

        // Handle premium purchase
        try
        {
            if (_adService != null)
            {
                var purchased = await _adService.PurchasePremiumAsync();
                if (purchased)
                {
                    IsPremium = true;
                    await SaveSettingsAsync();
                    
                    if (Application.Current?.Windows.Count > 0)
                    {
                        await Application.Current.Windows[0].Page!.DisplayAlert(
                            "Premium Activated!", 
                            "Thank you for your purchase! Ads have been removed and all features unlocked! 🎉", 
                            "Enjoy!");
                    }
                }
            }
            else
            {
                // Fallback if ad service not available
                if (Application.Current?.Windows.Count > 0)
                {
                    var result = await Application.Current.Windows[0].Page!.DisplayAlert(
                        "Premium Purchase", 
                        "Would you like to purchase premium features for $2.99?\n\n• Remove all ads\n• Unlock all game modes\n• Support development", 
                        "Buy Now", "Maybe Later");
                    
                    if (result)
                    {
                        IsPremium = true;
                        await SaveSettingsAsync();
                        
                        await Application.Current.Windows[0].Page!.DisplayAlert(
                            "Premium Activated!", 
                            "Thank you for your purchase! Premium features unlocked! 🎉", 
                            "Enjoy!");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            if (Application.Current?.Windows.Count > 0)
            {
                await Application.Current.Windows[0].Page!.DisplayAlert(
                    "Purchase Error", 
                    $"Sorry, there was an issue with the purchase: {ex.Message}", 
                    "OK");
            }
        }
    }

    public async Task SaveSettingsAsync()
    {
        try
        {
            // Save to preferences
            Preferences.Set("SoundEffects", SoundEffectsEnabled);
            Preferences.Set("BackgroundMusic", BackgroundMusicEnabled);
            Preferences.Set("MasterVolume", MasterVolume);
            Preferences.Set("Difficulty", SelectedDifficulty);
            Preferences.Set("HapticFeedback", HapticFeedbackEnabled);
            Preferences.Set("ShowFpsCounter", ShowFpsCounter);
            Preferences.Set("IsPremium", IsPremium);

            if (Application.Current?.Windows.Count > 0)
            {
                await Application.Current.Windows[0].Page!.DisplayAlert(
                    "Settings Saved", 
                    "Your settings have been saved successfully! ✅", 
                    "OK");
            }
        }
        catch (Exception ex)
        {
            if (Application.Current?.Windows.Count > 0)
            {
                await Application.Current.Windows[0].Page!.DisplayAlert(
                    "Save Error", 
                    $"Error saving settings: {ex.Message}", 
                    "OK");
            }
        }
    }

    public async Task ResetSettingsAsync()
    {
        if (Application.Current?.Windows.Count > 0)
        {
            var result = await Application.Current.Windows[0].Page!.DisplayAlert(
                "Reset Settings", 
                "Are you sure you want to reset all settings to their default values?", 
                "Reset", "Cancel");

            if (result)
            {
                // Reset to defaults
                SoundEffectsEnabled = true;
                BackgroundMusicEnabled = true;
                MasterVolume = 80.0;
                SelectedDifficulty = "Medium";
                HapticFeedbackEnabled = true;
                ShowFpsCounter = false;
                // Don't reset premium status

                await SaveSettingsAsync();
            }
        }
    }

    public async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("//MenuPage");
    }

    private void LoadSettings()
    {
        // Load from preferences
        SoundEffectsEnabled = Preferences.Get("SoundEffects", true);
        BackgroundMusicEnabled = Preferences.Get("BackgroundMusic", true);
        MasterVolume = Preferences.Get("MasterVolume", 80.0);
        SelectedDifficulty = Preferences.Get("Difficulty", "Medium");
        HapticFeedbackEnabled = Preferences.Get("HapticFeedback", true);
        ShowFpsCounter = Preferences.Get("ShowFpsCounter", false);
        IsPremium = Preferences.Get("IsPremium", false);
    }
}
