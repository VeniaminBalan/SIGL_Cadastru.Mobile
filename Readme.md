# .NET MAUI Application Development Guide

## Using MVVM + CommunityToolkit + Markup (C#-Only UI)

This instruction set defines the **rules, patterns, and best practices** to follow when developing a .NET MAUI application using:

* **MVVM pattern**
* **CommunityToolkit.Mvvm** (for ViewModels & Commands)
* **CommunityToolkit.Maui** (controls/helpers)
* **CommunityToolkit.Maui.Markup** (C# Fluent UI instead of XAML)

---

# 1. Project Structure Rules

Organize your project cleanly to keep scalability and maintainability.

```
YourApp/
 ├── App.xaml.cs
 ├── MauiProgram.cs
 ├── Models/
 ├── Services/
 ├── ViewModels/
 └── Views/
```

### Rules

1. **Views = UI only**, never add logic.
2. **ViewModels = state + commands**, no UI code or navigation code tied to UI controls.
3. **Services = business logic**, data access, API calls.
4. Use **Dependency Injection for everything**.

---

# 2. Registration Rules (MauiProgram.cs)

All dependencies must be registered in one place.

### Use DI.cs for Organization

* Use a `DI.cs` class with extension methods to organize registrations:
  * `RegisterServices()` - for all services
  * `RegisterViewModels()` - for all ViewModels
  * `RegisterViews()` - for all Views/Pages
* Call these methods from `MauiProgram.cs` to keep it clean

### ViewModels

```
services.AddTransient<YourViewModel>();
```

* Always **Transient** so each page gets a fresh ViewModel.

### Pages (Views)

```
services.AddTransient<YourPage>();
```

* Use DI to auto-inject the ViewModel.

### Services

```
services.AddSingleton<YourService>();
```

* Services should be **Singletons**.

### Toolkit

```
builder.UseMauiCommunityToolkit();
builder.UseMauiCommunityToolkitMarkup();
```

---

# 3. MVVM Rules

## 3.1 ViewModel Rules

* Inherit from `ObservableObject`
* Use attributes:

  * `[ObservableProperty]` for bindable fields
  * `[RelayCommand]` or `[IAsyncRelayCommand]` for actions
* ViewModel **must not reference UI controls**.

**ViewModel structure:**

```
public partial class ExampleViewModel : ObservableObject
{
    [ObservableProperty] string title;
    [ObservableProperty] bool isBusy;

    [RelayCommand]
    async Task LoadDataAsync() { ... }
}
```

## 3.2 Model Rules

* Only simple data classes.
* Zero UI or ViewModel logic.

## 3.3 Service Rules

* Use `HttpClient` for API communication.
* Keep all API logic out of ViewModels.

---

# 4. Markup UI Rules (No XAML)

All UI must be created using **CommunityToolkit.Maui.Markup**.

### Naming Rules

* `YourPage.cs` not `YourPage.xaml`.
* Class inherits from `ContentPage`.
* UI built inside constructor.

### Basic Example Pattern

```
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
                new Label().Text("Hello").Center(),
                new Button().Text("Click Me")
                    .Bind(Button.CommandProperty, nameof(vm.ClickCommand))
            }
        };
    }
}
```

### Markup Rules

* Prefer fluent extensions: `.Text()`, `.FontSize()`, `.Center()`, `.Bind()`
* Never set `BindingContext` inside markup; only in constructor.
* Keep UI layout simple and readable.

---

# 5. Navigation Rules

* Use **Shell Navigation**.
* Register routes in `AppShell`.

```
Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
```

Navigate from ViewModel using:

```
await Shell.Current.GoToAsync(nameof(HomePage));
```

ViewModel **never** instantiates Pages manually.

---

# 6. Async + Command Rules

### Use async commands

```
[RelayCommand]
async Task LoginAsync() { ... }
```

### Avoid blocking UI

Never use `.Result` or `.Wait()`.

### Disable buttons while busy

```
[ObservableProperty] bool isBusy;
```

Bind in UI:

```
new Button()
    .Bind(Button.IsEnabledProperty, nameof(vm.IsBusy), convert: v => !v)
```

---

# 7. State Management Rules

* Use `[ObservableProperty]` for all state.
* Never manipulate UI controls directly.
* Use `WeakReferenceMessenger` for cross-ViewModel communication.

---

# 8. Error Handling + Validation Rules

* Never show alerts inside services.
* ViewModel handles UI messages.
* Use `DisplayAlert()` only in the ViewModel via `Shell.Current`.

---

# 9. Clean Architecture Rules

✓ Services do the work
✓ ViewModels orchestrate
✓ Views display
✓ Models represent data

**NO rule-breaking allowed:**
❌ View calling services directly
❌ ViewModel instantiating pages
❌ UI logic inside ViewModel
❌ Business logic inside View

---

# 10. Summary Ruleset

### Always

* MVVM separation
* Markup-only UI
* DI for everything
* CommunityToolkit for boilerplate reduction
* Async commands

### Never

* Add UI logic in ViewModels
* Mix XAML with Markup
* Instantiate pages manually
* Access services from Views

---

This guide defines all required rules for a clean, scalable MAUI application using **MVVM + CommunityToolkit + Markup UI**.
