using SistemaTurnos.WebApplication.WebApi.Controllers;
using Xunit;
using SistemaTurnos.Database.Enums;

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
                BusinessTypeId = BusinessType.Hairdressing,
                Email = "email@test.com",
                Address = "Address",
                Password = "aaa",
                City = 1
            });

            Assert.True(((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200);

            Assert.Equal(BusinessType.Hairdressing, ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value);
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
                Username = "email@test.com",
                Password = "1234"
            });

            Assert.NotNull(res.Token);
        }
    }
}
