using Moq;
using SistemaTurnos.WebApplication.WebApi.Controllers;
using System;
using Xunit;
using Microsoft.AspNetCore.Identity;
using SistemaTurnos.Database.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace ApiTests
{
    public class IdentityFixture
    {
        public static Mock<Microsoft.AspNetCore.Http.HttpContext> GetHttpContext()
        {
            var httpContext = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
            var items = new Dictionary<object, object>();
            items.Add("userId", 1);
            httpContext.Setup(c => c.Items).Returns(items);

            return httpContext;
        }

        public static Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var mockStore = Mock.Of<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(mockStore, null, null, null, null, null, null, null, null);

            mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockUserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockUserManager
                .Setup(x => x.Users)
                .Returns((new List<ApplicationUser> { new ApplicationUser() { Id = 123, Email = "email@test.com" } }).AsQueryable());

            mockUserManager
                .Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string>() {Roles.Administrator, Roles.AdministratorAndEmployee, Roles.AdministratorAndEmployeeAndClient, Roles.Client, Roles.Employee });

            return mockUserManager;
        }

       

        public static Mock<RoleManager<ApplicationRole>> GetMockRoleManager()
        {
            var mockRoleStore = Mock.Of<IRoleStore<ApplicationRole>>();
            var mockRoleManager = new Mock<RoleManager<ApplicationRole>>(mockRoleStore, null, null, null, null);
            mockRoleManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationRole>()))
                .ReturnsAsync(IdentityResult.Success);

            mockRoleManager
                .Setup(x => x.RoleExistsAsync(Roles.Administrator))
                .ReturnsAsync(true);

            mockRoleManager
                .Setup(x => x.RoleExistsAsync(Roles.Client))
                .ReturnsAsync(true);

            return mockRoleManager;
        }

        public static Mock<SignInManager<ApplicationUser>> GetMockSignInManager(UserManager<ApplicationUser> userManager)
        {
            var signInManager = new Mock<SignInManager<ApplicationUser>>(
                userManager,
                Mock.Of<IHttpContextAccessor>(),
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                null);

            signInManager
                .Setup(x => x.CanSignInAsync(Mock.Of<ApplicationUser>()))
                .ReturnsAsync(true);

            signInManager
                .Setup(x => x.PasswordSignInAsync(It.IsAny<string>(),
                                      It.IsAny<string>(),
                                      It.IsAny<bool>(),
                                      It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            return signInManager;
        }

        public static IConfiguration GetConfiguration()
        {
            var iConfig = new Mock<IConfiguration>();
            iConfig.SetupGet(x => x["JwtIssuer"]).Returns("user");
            iConfig.SetupGet(x => x["JwtKey"]).Returns("kFqEVVkK4BPh7IIzuaBjFQ");
            iConfig.SetupGet(x => x["JwtExpireDays"]).Returns("2");

            return iConfig.Object;
        }
    }
}
