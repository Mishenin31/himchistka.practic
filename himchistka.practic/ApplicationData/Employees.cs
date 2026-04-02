using himchistka.practic.ApplicationData;
using System;
using System.Collections.Generic;

namespace Cleaners.ApplicationData
{
    public class Employees
    {
        public int EmployeeID { get; set; }
        public int? CityID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }

        // Поля для авторизации
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? LastLoginDate { get; set; }

        // Навигационные свойства
        public virtual Cities City { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
        public virtual ICollection<Receipts> Receipts { get; set; }
        public virtual ICollection<OrderStages> OrderStages { get; set; }
        public virtual ICollection<Branches> ManagedBranches { get; set; }
    }
}