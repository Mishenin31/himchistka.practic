using System;
using himchistka.practic.Models;

namespace himchistka.practic.Services;

public class SessionService
{
    public event EventHandler? SessionChanged;

    public User? CurrentUser { get; private set; }

    public bool IsAuthenticated => CurrentUser is not null;

    public void Login(User user)
    {
        CurrentUser = user;
        SessionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Logout()
    {
        CurrentUser = null;
        SessionChanged?.Invoke(this, EventArgs.Empty);
    }
}
