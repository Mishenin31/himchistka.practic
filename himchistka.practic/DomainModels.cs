using System;

namespace himchistka.practic
{
    public enum UserRole
    {
        User,
        Manager,
        Admin
    }

    public sealed class UserAccount
    {
        public string FullName { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
    }

    public sealed class OrderRecord
    {
        public int Id { get; set; }
        public string ClientFullName { get; set; }
        public string ServiceName { get; set; }
        public DateTime DateReceived { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
    }

    public sealed class ClientRecord
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string LoyaltyLevel { get; set; }
    }

    public sealed class ServiceCatalogRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Duration { get; set; }
        public decimal BasePrice { get; set; }
    }
}
