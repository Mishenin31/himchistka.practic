using System;
using System.Linq;
using himchistka.practic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cleaners.Tests;

[TestClass]
public class BusinessLogicTests
{
    [TestMethod]
    public void Register_ShouldCreateUser_WhenDataIsValid()
    {
        var auth = new AuthService();
        var user = auth.Register("Тестовый Пользователь", "newuser", "newuser123", UserRole.User);

        Assert.IsNotNull(user);
        Assert.AreEqual("newuser", user.Login);
    }

    [TestMethod]
    public void Login_ShouldReturnNull_WhenPasswordIsInvalid()
    {
        var auth = new AuthService();
        var user = auth.Login("admin", "wrong-password");

        Assert.IsNull(user);
    }

    [TestMethod]
    public void Validation_ShouldRejectShortLogin()
    {
        Assert.IsFalse(ValidationService.IsValidLogin("abc"));
    }

    [TestMethod]
    public void AddOrder_ShouldIncreaseCollectionSize()
    {
        var repo = new OrderRepository();
        var before = repo.Orders.Count;

        repo.AddOrder(new OrderRecord
        {
            ClientFullName = "Тест",
            ServiceName = "Химчистка",
            DateReceived = DateTime.Today,
            TotalPrice = 500,
            Status = "Новый"
        });

        Assert.AreEqual(before + 1, repo.Orders.Count);
    }

    [TestMethod]
    public void DeleteOrder_ShouldRemoveExistingOrder()
    {
        var repo = new OrderRepository();
        var firstId = repo.Orders.First().Id;

        repo.DeleteOrder(firstId);

        Assert.IsFalse(repo.Orders.Any(x => x.Id == firstId));
    }
}
