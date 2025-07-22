using BalloonPopper.Maui.PageModels;

namespace BalloonPopper.Maui.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsPageModel _pageModel;

    public SettingsPage(SettingsPageModel pageModel)
    {
        InitializeComponent();
        _pageModel = pageModel;
        BindingContext = _pageModel;
    }

    private async void OnPremiumClicked(object sender, EventArgs e)
    {
        await _pageModel.HandlePremiumPurchaseAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await _pageModel.SaveSettingsAsync();
    }

    private async void OnResetClicked(object sender, EventArgs e)
    {
        await _pageModel.ResetSettingsAsync();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await _pageModel.NavigateBackAsync();
    }
}
