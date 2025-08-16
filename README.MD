# Balloon Popper Game

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-9.0-purple)](https://dotnet.microsoft.com/apps/maui)
[![iOS](https://img.shields.io/badge/iOS-15%2B-blue)](https://developer.apple.com/ios/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

A casual balloon-popping mobile game built with .NET MAUI for iOS devices. Features simple yet addictive gameplay where players pop balloons to score points, with monetization through in-game advertisements.

## 🚀 Quick Start

### Prerequisites

- Visual Studio 2022 17.8+ or Visual Studio for Mac
- .NET 9.0 SDK
- iOS development workload
- Xcode 15+ (for iOS deployment)

### Building and Running

```bash
# Clone the repository
git clone https://github.com/JeffreyFerreiras/BalloonPopper.git
cd BalloonPopper

# Restore packages
dotnet restore

# Build for iOS
dotnet build src/BalloonPopper.Maui/BalloonPopper.Maui.csproj -f net9.0-ios

# Run tests
dotnet test tests/BalloonPopper.Maui.UnitTests/
```

## 🎮 Game Features

### Core Mechanics

- Balloons float upwards from the bottom of the screen
- Players tap balloons to pop them and score points
- Different balloon colors grant varying points
- Special balloons offer power-ups or bonus points

### Game Modes

- **Classic Mode:** Timed sessions of 60 seconds to pop as many balloons as possible
- **Endless Mode:** Continuous gameplay until a certain number of balloons are missed

### Power-Ups and Bonuses

- **Time Freeze:** Temporarily slows balloon movement
- **Score Multiplier:** Double points for limited duration
- **Mega Pop:** Instantly pops all visible balloons

## 💰 Monetization

### Ad Integration

- **Banner Ads:** Displayed at the bottom of gameplay screens
- **Interstitial Ads:** Shown after completing every 3 gameplay sessions
- **Rewarded Video Ads:** Users can watch ads to gain power-ups, extra time, or second chances

### Premium Features

- One-time purchase to remove ads permanently

## 🛠️ Technical Stack

- **Platform:** iOS 15+
- **Framework:** .NET MAUI 9.0
- **Language:** C#
- **Architecture:** MVVM with Dependency Injection
- **Ads SDK:** Google AdMob
- **Testing:** xUnit with Moq for mocking

## 📁 Project Structure

```text
BalloonPopper.sln
README.MD
builds/                         # build artifacts and CI helpers
docs/                           # design docs, maps and plans
src/
├── BalloonPopper.Maui/         # Main .NET MAUI application (UI + app shell)
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── AppShell.xaml
│   ├── AppShell.xaml.cs
│   ├── GlobalUsings.cs
│   ├── MauiProgram.cs
│   ├── PageModels/              # ViewModels (MVVM)
│   ├── Pages/                   # XAML pages and code-behind
│   ├── Platforms/               # platform-specific code
│   ├── Resources/               # images, fonts, styles
│   ├── Services/                # app-specific services (DI registrations)
│   └── Utilities/               # helpers and extensions
├── BalloonPopper.Models/        # Domain models and simple data types
│   ├── Balloon.cs
│   ├── BalloonSpawnConfig.cs
│   ├── DifficultyConfig.cs
│   ├── GameState.cs
│   └── Point.cs
├── BalloonPopper.Services/      # Core game services and engines
│   ├── BalloonInteractionService.cs
│   ├── BalloonSpawner.cs
│   ├── DifficultyManager.cs
│   ├── GameEngine.cs
│   ├── GameStateManager.cs
│   ├── SimpleGameEngine.cs
│   └── AbstractionsImpl/        # concrete implementations for abstractions
└── BalloonPopper.Services.Abstractions/  # service interfaces and contracts
     ├── IAdService.cs
     ├── IBalloonInteractionService.cs
     ├── IBalloonSpawner.cs
     ├── IDifficultyManager.cs
     ├── IGameEngine.cs
     └── IGameStateService.cs

tests/
├── BalloonPopper.Maui.UnitTests/
├── BalloonPopper.Models.UnitTests/
└── BalloonPopper.Services.UnitTests/

```

## 🏗️ Architecture

The project follows SOLID principles and clean architecture patterns:

- **Dependency Injection:** All services are injected through constructor injection
- **Repository Pattern:** Data access abstracted through repository interfaces  
- **MVVM Pattern:** Clear separation between UI and business logic
- **Interface Segregation:** Small, focused interfaces for better testability
- **Pure Functions:** Game logic implemented as pure functions where possible

## 🧪 Testing

Run the test suite to ensure code quality:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test categories
dotnet test --filter Category=Unit
dotnet test --filter Category=Integration
```

Current test coverage: >90% for core game mechanics

## 📱 Target Audience

- Casual gamers of all ages
- Families looking for simple, engaging gameplay
- Users who enjoy quick, pick-up-and-play games

## 🎨 UI/UX Design

- Simple, vibrant, and intuitive UI suitable for all ages
- Clear button labels and responsive touch feedback
- Minimalistic menus to navigate between game modes, settings, and store
- Accessibility support (VoiceOver, Dynamic Type)
- Responsive design for various iOS device sizes

## 📊 Analytics & Performance

- Integrated analytics to monitor user engagement and ad performance
- Performance optimization for smooth gameplay on various iOS devices
- Memory usage monitoring and optimization
- Crash reporting and error tracking

## 🚢 Deployment

### iOS App Store

```bash
# Build for release
dotnet publish src/BalloonPopper.Maui/BalloonPopper.Maui.csproj -f net9.0-ios -c Release

# Archive for App Store submission
# (Use Xcode or Visual Studio for final archive and submission)
```

### CI/CD Pipeline

- Automated builds on every commit
- Unit test execution in pipeline
- Code quality checks and coverage reports
- Automated deployment to TestFlight for beta testing

## 📋 Development Roadmap

- [x] Core game mechanics implementation
- [x] MVVM architecture setup
- [x] Basic UI implementation
- [ ] AdMob integration
- [ ] In-app purchase implementation
- [ ] Advanced animations and effects
- [ ] Performance optimization
- [ ] App Store submission

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/new-feature`)
3. Follow SOLID principles and dependency injection patterns
4. Ensure >90% unit test coverage for new code
5. Run all tests before committing
6. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For technical issues or questions:

- Create an issue in this repository
- Follow the bug report template
- Include device information and reproduction steps
