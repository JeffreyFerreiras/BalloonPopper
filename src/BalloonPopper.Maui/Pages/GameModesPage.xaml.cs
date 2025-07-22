using BalloonPopper.Maui.PageModels;

namespace BalloonPopper.Maui.Pages;

public partial class GameModesPage : ContentPage
{
    private readonly GameModesPageModel _pageModel;

    public GameModesPage(GameModesPageModel pageModel)
    {
        InitializeComponent();
        _pageModel = pageModel;
        BindingContext = _pageModel;
    }

    private async void OnClassicModeClicked(object sender, EventArgs e)
    {
        await _pageModel.StartClassicModeAsync();
    }

    private async void OnTimeAttackClicked(object sender, EventArgs e)
    {
        await _pageModel.StartTimeAttackModeAsync();
    }

    private async void OnEndlessModeClicked(object sender, EventArgs e)
    {
        await _pageModel.StartEndlessModeAsync();
    }

    private async void OnPrecisionModeClicked(object sender, EventArgs e)
    {
        await _pageModel.StartPrecisionModeAsync();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await _pageModel.NavigateBackAsync();
    }
}
