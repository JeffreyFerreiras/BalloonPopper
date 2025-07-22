using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using BalloonPopper.Models;
using BalloonPopper.Services;

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
    				handlers.AddHandler<Microsoft.Maui.Controls.CollectionView, Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2>();
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
            builder.Services.AddSingleton<MainPageModel>();

            // Game services following SOLID principles with dependency injection
            builder.Services.AddSingleton<BalloonSpawnConfig>();
            builder.Services.AddSingleton<DifficultyConfig>();
            builder.Services.AddSingleton<IBalloonSpawner, BalloonSpawner>();
            builder.Services.AddSingleton<IGameStateManager, GameStateManager>();
            builder.Services.AddSingleton<IBalloonInteractionService, BalloonInteractionService>();
            builder.Services.AddSingleton<IDifficultyManager, DifficultyManager>();
            builder.Services.AddSingleton<IScoringService, ScoringService>();
            builder.Services.AddSingleton<IGameEngine, GameEngine>();


            return builder.Build();
        }
    }
}
