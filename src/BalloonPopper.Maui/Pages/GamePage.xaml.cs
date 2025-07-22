using BalloonPopper.Maui.PageModels;

namespace BalloonPopper.Maui.Pages;

[QueryProperty(nameof(GameMode), "mode")]
public partial class GamePage : ContentPage
{
    private readonly GamePageModel _pageModel;

    public GamePage(GamePageModel pageModel)
    {
        InitializeComponent();
        _pageModel = pageModel;
        BindingContext = _pageModel;
    }

    public string GameMode
    {
        set => _pageModel.SetGameMode(value);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _pageModel.StartGame();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _pageModel.PauseGame();
    }

    private void OnBackToMenuClicked(object sender, EventArgs e)
    {
        _pageModel.NavigateToMainMenu();
    }

    private void OnPauseClicked(object sender, EventArgs e)
    {
        _pageModel.TogglePause();
        PauseOverlay.IsVisible = _pageModel.IsPaused;
    }

    private void OnResumeClicked(object sender, EventArgs e)
    {
        _pageModel.ResumeGame();
        PauseOverlay.IsVisible = false;
    }

    private void OnRestartClicked(object sender, EventArgs e)
    {
        _pageModel.RestartGame();
        PauseOverlay.IsVisible = false;
    }

    private void OnMainMenuClicked(object sender, EventArgs e)
    {
        _pageModel.NavigateToMainMenu();
    }
}
