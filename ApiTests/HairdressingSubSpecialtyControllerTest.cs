using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.WebApplication.WebApi.Controllers;
using SistemaTurnos.WebApplication.WebApi.Services;
using Xunit;

namespace ApiTests
{
    public class HairdressingSubSpecialtyControllerTest
    {
        [Fact]
        public void GetAll_Ok()
        {
            var dbContext = DbContextFixture.GetDbContext();

            var httpContext = IdentityFixture.GetHttpContext();

            var controller = new HairdressingSubSpecialtyController(dbContext)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                },

                _service = new BusinessPlaceService()
            };

            var res = controller.GetAll();
            Assert.True(res.Count == 2);
        }
    }
}
