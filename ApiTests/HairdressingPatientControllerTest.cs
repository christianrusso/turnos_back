using SistemaTurnos.WebApplication.WebApi.Controllers;
using Xunit;
using SistemaTurnos.WebApplication.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiTests
{
    public class HairdressingPatientControllerTest
    {
        [Fact]
        public void CreateClient()
        {
            var httpContext = IdentityFixture.GetHttpContext();
            var dbContext = DbContextFixture.GetDbContext();
            var userMng = IdentityFixture.GetMockUserManager();
            var roleMng = IdentityFixture.GetMockRoleManager();
            var controller = new HairdressingPatientController(userMng.Object, roleMng.Object, dbContext);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            controller._service = new BusinessPlaceService();

            var res = controller.Add(new SistemaTurnos.WebApplication.WebApi.Dto.HairdressingPatient.AddHairdressingPatientDto()
            {
                Address = "address",
                ClientId = 1,
                Dni = "999999998",
                Email="email@test.com",
                FirstName = "firstName",
                LastName = "lastname",
                PhoneNumber = ""
            });

            Assert.True(((ObjectResult)res).StatusCode == 200);

            Assert.True((int)((ObjectResult)res).Value > 0);
        }
    }
}
