using Moq;
using SistemaTurnos.WebApplication.WebApi.Controllers;
using System;
using Xunit;
using Microsoft.AspNetCore.Identity;
using SistemaTurnos.WebApplication.Database.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.WebApplication.Database;
using System.Collections.Generic;
using System.Linq;

namespace ApiTests
{
    public class AccountControllerTest
    {
        [Fact]
        public void Hairdressing_Register_Ok()
        {
            //Arrange
            
            var mockUserManager = IdentityFixture.GetMockUserManager();

            var mockRoleManager = IdentityFixture.GetMockRoleManager();

            var signInManager = IdentityFixture.GetMockSignInManager(mockUserManager.Object);

            var iConfig = IdentityFixture.GetConfiguration();

            var context = DbContextFixture.GetDbContext();

            var accountCtrl = new AccountController(mockUserManager.Object, mockRoleManager.Object, signInManager.Object, iConfig, context);

            var res = accountCtrl.Register(new SistemaTurnos.WebApplication.WebApi.Dto.Account.RegisterAccountDto()
            {
                BusinessType = SistemaTurnos.WebApplication.Database.Enums.BusinessType.Hairdressing,
                Email = "email@test.com",
                Address = "Address",
                Password = "aaa",
                City = 1
            });

            Assert.True(((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200);

            Assert.Equal(SistemaTurnos.WebApplication.Database.Enums.BusinessType.Hairdressing, ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value);
        }

        [Fact]
        public void Hairdressing_Login_Ok()
        {
            //Arrange

            var mockUserManager = IdentityFixture.GetMockUserManager();

            var mockRoleManager = IdentityFixture.GetMockRoleManager();

            var signInManager = IdentityFixture.GetMockSignInManager(mockUserManager.Object);

            var iConfig = IdentityFixture.GetConfiguration();

            var context = DbContextFixture.GetDbContext();

            var accountCtrl = new AccountController(mockUserManager.Object, mockRoleManager.Object, signInManager.Object, iConfig, context);

            var res = accountCtrl.Login(new SistemaTurnos.WebApplication.WebApi.Dto.Account.LoginAccountDto()
            {
                BusinessType = SistemaTurnos.WebApplication.Database.Enums.BusinessType.Hairdressing,
                Email = "email@test.com",
                Password = "1234"
            });

            Assert.NotNull(res.Token);
        }
    }
}
