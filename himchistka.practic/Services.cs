using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace himchistka.practic
{
    public static class ValidationService
    {
        public static bool IsValidLogin(string login)
        {
            return !string.IsNullOrWhiteSpace(login) && login.Length >= 4;
        }

        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 6;
        }

        public static bool IsValidOrder(OrderRecord order)
        {
            return order != null
                   && !string.IsNullOrWhiteSpace(order.ClientFullName)
                   && !string.IsNullOrWhiteSpace(order.ServiceName)
                   && order.TotalPrice >= 0;
        }
    }

    public sealed class AuthService
    {
        private readonly List<UserAccount> _users = new List<UserAccount>
        {
            new UserAccount { FullName = "Администратор", Login = "admin", PasswordHash = Hash("admin123"), Role = UserRole.Admin },
            new UserAccount { FullName = "Менеджер", Login = "manager", PasswordHash = Hash("manager123"), Role = UserRole.Manager },
            new UserAccount { FullName = "Пользователь", Login = "user", PasswordHash = Hash("user123"), Role = UserRole.User }
        };

        public UserAccount Register(string fullName, string login, string password, UserRole role)
        {
            if (!ValidationService.IsValidLogin(login))
            {
                throw new InvalidOperationException("Логин должен содержать минимум 4 символа.");
            }

            if (!ValidationService.IsValidPassword(password))
            {
                throw new InvalidOperationException("Пароль должен содержать минимум 6 символов.");
            }

            if (_users.Any(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Пользователь с таким логином уже существует.");
            }

            var account = new UserAccount
            {
                FullName = fullName,
                Login = login,
                PasswordHash = Hash(password),
                Role = role
            };

            _users.Add(account);
            return account;
        }

        public UserAccount Login(string login, string password)
        {
            var hash = Hash(password ?? string.Empty);
            return _users.FirstOrDefault(x => x.Login.Equals(login ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                                              && x.PasswordHash == hash);
        }

        private static string Hash(string raw)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw ?? string.Empty));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
