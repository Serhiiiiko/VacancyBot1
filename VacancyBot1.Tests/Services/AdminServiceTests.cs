using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacancyBot1.Data;
using VacancyBot1.Models;
using VacancyBot1.Services;

namespace VacancyBot1.Tests.Services;
public class AdminServiceTests
{
    [Fact]
    public void IsAdmin_ShouldReturnTrue_WhenUserIsAdmin()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "AdminTestDb")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Admins.Add(new Admin { Id = 1, TelegramId = 611091030, Email = "example@gmail.com", IsSuperAdmin = true, TelegramUsername = "sejoju" });
            context.SaveChanges();

            var adminService = new AdminService(null, context);

            // Act
            var result = adminService.IsAdmin(611091030);

            // Assert
            Assert.True(result);
        }
    }

    [Fact]
    public void IsAdmin_ShouldReturnFalse_WhenUserIsNotAdmin()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "AdminTestDb2")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Admins.Add(new Admin { Id = 1, TelegramId = 611091030 , Email = "example@gmail.com", IsSuperAdmin = true, TelegramUsername = "sejoju"});
            context.SaveChanges();

            var adminService = new AdminService(null, context);

            // Act
            var result = adminService.IsAdmin(123456789);

            // Assert
            Assert.False(result);
        }
    }
}