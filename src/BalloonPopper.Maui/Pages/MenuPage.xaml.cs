using BalloonPopper.Maui.PageModels;

namespace BalloonPopper.Maui.Pages;

public partial class MenuPage : ContentPage
{
    private readonly MenuPageModel _pageModel;

    public MenuPage(MenuPageModel pageModel)
    {
        InitializeComponent();
        _pageModel = pageModel;
        BindingContext = _pageModel;
    }

    private async void OnStartGameClicked(object sender, EventArgs e)
    {
        await _pageModel.StartGameAsync();
    }

    private async void OnGameModesClicked(object sender, EventArgs e)
    {
        await _pageModel.NavigateToGameModesAsync();
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await _pageModel.NavigateToSettingsAsync();
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await _pageModel.ShowAboutAsync();
    }
}
