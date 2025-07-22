using BalloonPopper.Models;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;

namespace BalloonPopper.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
#if IOS || MACCATALYST
                    handlers.AddHandler<
                        Microsoft.Maui.Controls.CollectionView,
                        Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2
                    >();
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            // Existing services
            builder.Services.AddSingleton<ModalErrorHandler>();

            // Page Models
            builder.Services.AddTransient<MainPageModel>();
            builder.Services.AddTransient<MenuPageModel>();
            builder.Services.AddTransient<GameModesPageModel>();
            builder.Services.AddTransient<SettingsPageModel>();
            builder.Services.AddTransient<GamePageModel>();

            // Pages
            builder.Services.AddTransient<Pages.MainPage>();
            builder.Services.AddTransient<Pages.MenuPage>();
            builder.Services.AddTransient<Pages.GameModesPage>();
            builder.Services.AddTransient<Pages.SettingsPage>();
            builder.Services.AddTransient<Pages.GamePage>();

            // Game model configuration
            builder.Services.AddSingleton<BalloonSpawnConfig>();
            builder.Services.AddSingleton<DifficultyConfig>();

            // Register interfaces with concrete implementations (Brian will implement these)
            // builder.Services.AddSingleton<IBalloonSpawner, BalloonSpawner>();
            // builder.Services.AddSingleton<IGameStateService, GameStateManager>();
            // builder.Services.AddSingleton<IBalloonInteractionService, BalloonInteractionService>();
            // builder.Services.AddSingleton<IDifficultyManager, DifficultyManager>();
            // builder.Services.AddSingleton<IGameEngine, GameEngine>();

            // Ad service placeholder (David will implement)
            // builder.Services.AddSingleton<IAdService, AdService>();

            return builder.Build();
        }
    }
}
