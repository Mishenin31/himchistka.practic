using System;
using System.Windows.Controls;
using himchistka.practic.Models;
using himchistka.practic.Views;
using Microsoft.Extensions.DependencyInjection;

namespace himchistka.practic.Services;

public class NavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SessionService _sessionService;
    private Frame? _frame;

    public NavigationService(IServiceProvider serviceProvider, SessionService sessionService)
    {
        _serviceProvider = serviceProvider;
        _sessionService = sessionService;
    }

    public void SetFrame(Frame frame) => _frame = frame;

    public void Navigate<TPage>() where TPage : Page
    {
        EnsureAccess(typeof(TPage));
        var page = _serviceProvider.GetRequiredService<TPage>();
        _frame?.Navigate(page);
    }

    public bool CanGoBack => _frame?.CanGoBack == true;

    public void GoBack()
    {
        if (CanGoBack)
        {
            _frame?.GoBack();
        }
    }

    private void EnsureAccess(Type pageType)
    {
        var currentUser = _sessionService.CurrentUser;

        if (pageType == typeof(ProductsPage) || pageType == typeof(UsersPage))
        {
            if (currentUser is null || !currentUser.IsAdmin)
            {
                throw new UnauthorizedAccessException("Только администратор может открыть эту страницу.");
            }
        }
    }
}
