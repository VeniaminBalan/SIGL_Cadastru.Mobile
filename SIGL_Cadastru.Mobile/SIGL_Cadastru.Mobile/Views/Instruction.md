# ✅ Rules for Creating a New MAUI Page Using Markup + MVVM

Use this checklist for every new page you add.

## 1. Create a Dedicated Folder Structure

```
/Pages
    /Login
        LoginPage.cs
        LoginViewModel.cs
        LoginModel.cs (optional)
```

**Rules:**
- Every page gets its own subfolder.
- Name files consistently: `XPage`, `XViewModel`, `XModel`.
- Never mix UI code with business logic.

## 2. ViewModels Must:

### ✔ Inherit from ObservableObject
```csharp
public partial class LoginViewModel : ObservableObject { }
```

### ✔ Use [ObservableProperty] and [RelayCommand]

Never write backing fields manually.

```csharp
public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty] 
    string username;

    [ObservableProperty]
    string password;

    [RelayCommand]
    async Task LoginAsync() { ... }
}
```

### ✔ Constructor injection for services
```csharp
public LoginViewModel(IAuthService authService)
{
    _authService = authService;
}
```

## 3. Register ViewModels and Pages in DI (MauiProgram.cs)

### ✔ Add Views and ViewModels
```csharp
builder.Services.AddTransient<LoginPage>();
builder.Services.AddTransient<LoginViewModel>();
```

### ✔ Add Services
```csharp
builder.Services.AddSingleton<IAuthService, AuthService>();
```

### ❌ Never instantiate ViewModels manually:
```csharp
// WRONG
var vm = new LoginViewModel();
```

### ✔ Always let DI create the page & VM
```csharp
// CORRECT
Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
```

## 4. Page Class Must:

✔ Inherit from `ContentPage`  
✔ Accept the ViewModel through constructor injection  
✔ Set `BindingContext = viewModel`  
✔ Build UI with markup extension (`Content = new VerticalStackLayout()...`)

```csharp
public class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        BindingContext = vm;

        Content = new VerticalStackLayout
        {
            Spacing = 12,
            Padding = 20
        }
        .Children(
            new Label()
                .Text("Login")
                .FontSize(32)
                .CenterHorizontal(),

            new Entry()
                .Placeholder("Username")
                .Bind(Entry.TextProperty, nameof(vm.Username)),

            new Entry()
                .Placeholder("Password")
                .IsPassword(true)
                .Bind(Entry.TextProperty, nameof(vm.Password)),

            new Button()
                .Text("Login")
                .BindCommand(nameof(vm.LoginCommand))
        );
    }
}
```

## 5. All Bindings Must Use nameof(vm.Property)

Never write plain strings.

### ✔ Safe:
```csharp
.Bind(Entry.TextProperty, nameof(vm.Username))
```

### ❌ Unsafe:
```csharp
.Bind(Entry.TextProperty, "Username")
```

## 6. Commands Must Be Async

✔ Always use `[RelayCommand] async Task`  
✔ Never use `async void` except event handlers

## 7. UI Must Not Block

✔ All long actions wrapped in async commands  
✔ Use an `IsBusy` flag when needed  
✔ Use markup bindings like:

```csharp
new ActivityIndicator()
    .Bind(IsVisibleProperty, nameof(vm.IsBusy))
    .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
```

## 8. Navigation Rules

### ✔ MVVM navigation always uses Shell + routes
```csharp
await Shell.Current.GoToAsync(nameof(HomePage));
```

### ✔ Register every page route in MauiProgram or AppShell
```csharp
Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
```

## 9. No Code-Behind Logic Except Creation

Only constructor + layout allowed.

### ❌ NO:
- Click handlers
- Business logic
- State logic
- Navigation logic

### ✔ ONLY:
- Set BindingContext
- Construct UI with Markup

## 10. Always Use Markup Chains

Never use XAML in a markup-first project.

### ✔ Good:
```csharp
new Label().Text("Hello").FontSize(24)
```

### ❌ Bad:
```xaml
<Label Text="Hello" />
```
