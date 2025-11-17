# SIGL Cadastru Mobile - AI Agent Instructions

## Architecture Overview

This is a .NET MAUI cross-platform mobile app (Android/iOS/Windows/MacCatalyst) using **strict MVVM + C# Markup** (zero XAML for UI). Authentication is via Keycloak/OIDC using Duende.IdentityModel.

**Key Constraint**: All UI must be built with **CommunityToolkit.Maui.Markup** fluent C# syntax - never generate XAML UI. The only XAML files are `App.xaml`, `AppShell.xaml`, and resource dictionaries.

## Project Structure

```
SIGL_Cadastru.Mobile/
├── MauiProgram.cs          # Entry point, calls DI extension methods
├── DI.cs                   # Centralized DI registration with extension methods
├── AppShell.xaml           # Shell navigation container (XAML only for Shell)
├── Models/                 # Data classes only
├── Services/               # Business logic, API calls (Singletons)
├── ViewModels/             # MVVM state + commands (Transient)
└── Views/                  # UI in C# Markup (Transient)
```

## Critical Patterns

### 1. DI Registration Pattern (DI.cs)

ALL registrations use extension methods on `IServiceCollection`:

```csharp
extension(IServiceCollection services)
{
    public void RegisterServices() { 
        services.AddSingleton<YourService>(); 
    }
    public void RegisterViewModels() { 
        services.AddTransient<YourViewModel>(); 
    }
    public void RegisterViews() { 
        services.AddTransient<YourPage>(); 
    }
}
```

**Never** register in `MauiProgram.cs` directly - always add to the appropriate extension method in `DI.cs`.

### 2. C# Markup UI Pattern (Views/)

Pages are pure C# classes inheriting `ContentPage`. ViewModel is injected via constructor:

```csharp
public class ExamplePage : ContentPage
{
    public ExamplePage(ExampleViewModel vm)
    {
        BindingContext = vm;

        Content = new VerticalStackLayout
        {
            Spacing = 10,
            Children =
            {
                new Label().Text("Title").FontSize(18).Center(),
                new Button()
                    .Text("Action")
                    .Bind(Button.CommandProperty, nameof(vm.ActionCommand))
            }
        };
    }
}
```

**Required imports**: `using CommunityToolkit.Maui.Markup;`

**Common fluent methods**: `.Text()`, `.FontSize()`, `.Center()`, `.Bind()`, `.Padding()`, `.Spacing()`

### 3. ViewModel Pattern

Use CommunityToolkit.Mvvm source generators:

```csharp
public partial class ExampleViewModel : ObservableObject
{
    [ObservableProperty] string title;      // Generates Title property
    [ObservableProperty] bool isBusy;

    [RelayCommand]
    async Task LoadData() { ... }           // Generates LoadDataCommand
}
```

Constructor injection for services:

```csharp
public ExampleViewModel(YourService service) { _service = service; }
```

### 4. Keycloak Authentication Flow

- `KeycloakAuthService` wraps `OidcClient` (Duende)
- `WebAuthenticatorBrowser` bridges MAUI's `WebAuthenticator` to OIDC
- Configuration in `DI.cs`: Authority = `https://auth.vbtm.live/realms/sigl-dev`
- Custom URL scheme: `sigl.mobile://callback`

Login/Logout pattern:
```csharp
await _authService.LoginAsync();          // Opens browser, returns bool
string token = _authService.AccessToken;
await _authService.RefreshAsync();
await _authService.LogoutAsync();
```

## Developer Workflows

### Building
Standard .NET MAUI build - target specific platform in csproj `TargetFrameworks`:
- `net10.0-android` / `net10.0-ios` / `net10.0-maccatalyst` / `net10.0-windows10.0.19041.0`

### Navigation
Shell-based navigation. Register routes in `AppShell.xaml` or code-behind:
```csharp
Routing.RegisterRoute(nameof(DetailPage), typeof(DetailPage));
```

Navigate from ViewModels:
```csharp
await Shell.Current.GoToAsync(nameof(DetailPage));
```

### Adding New Features

1. **Model**: Create in `Models/` (data class only)
2. **Service**: Create in `Services/`, register in `DI.cs` → `RegisterServices()` as `AddSingleton`
3. **ViewModel**: Create in `ViewModels/`, inherit `ObservableObject`, use `[ObservableProperty]` and `[RelayCommand]`, register in `DI.cs` → `RegisterViewModels()` as `AddTransient`
4. **View**: Create in `Views/` using C# Markup pattern, inject ViewModel, register in `DI.cs` → `RegisterViews()` as `AddTransient`

## Package Dependencies

- `CommunityToolkit.Maui` (v13.0.0) - Controls/helpers
- `CommunityToolkit.Maui.Markup` (v7.0.0) - C# fluent UI
- `CommunityToolkit.Mvvm` (v8.4.0) - Source generators for MVVM
- `Duende.IdentityModel.OidcClient` (v6.0.1) - OIDC/OAuth2

## Anti-Patterns to Avoid

❌ XAML UI in Views (except App.xaml, AppShell.xaml, resource dictionaries)  
❌ Direct service registration in `MauiProgram.cs` (use `DI.cs` extensions)  
❌ ViewModels instantiating Pages  
❌ Views calling Services directly  
❌ Manual property change notifications (use `[ObservableProperty]`)  
❌ Synchronous `.Result` or `.Wait()` on async tasks

## References

See `Readme.md` for complete MVVM + Markup ruleset and architectural guidelines.
