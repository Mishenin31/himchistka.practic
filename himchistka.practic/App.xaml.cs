using System;
using System.Windows;
using himchistka.practic.Services;
using himchistka.practic.ViewModels;
using himchistka.practic.Views;
using Microsoft.Extensions.DependencyInjection;

namespace himchistka.practic;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        Services = services.BuildServiceProvider();

        var mainWindow = Services.GetRequiredService<MainWindow>();
        MainWindow = mainWindow;
        mainWindow.Show();

        var navigation = Services.GetRequiredService<NavigationService>();
        navigation.Navigate<LoginPage>();

        base.OnStartup(e);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<SessionService>();
        services.AddSingleton<NavigationService>();
        services.AddSingleton<CartService>();

        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();

        services.AddTransient<LoginPage>();
        services.AddTransient<RegisterPage>();
        services.AddTransient<CatalogPage>();
        services.AddTransient<CartPage>();
        services.AddTransient<ProductsPage>();
        services.AddTransient<UsersPage>();
        services.AddTransient<CheckoutPage>();
        services.AddTransient<OrderPage>();

        services.AddTransient<LoginPageViewModel>();
        services.AddTransient<RegisterPageViewModel>();
        services.AddTransient<CatalogPageViewModel>();
        services.AddTransient<CartPageViewModel>();
        services.AddTransient<ProductsPageViewModel>();
        services.AddTransient<UsersPageViewModel>();
        services.AddTransient<CheckoutPageViewModel>();
        services.AddTransient<OrderPageViewModel>();
    }
}
