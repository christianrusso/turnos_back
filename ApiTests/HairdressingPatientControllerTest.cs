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

            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext();
            controller.ControllerContext.HttpContext = httpContext.Object;
            controller._service = new SistemaTurnos.WebApplication.WebApi.Services.BusinessPlaceService(httpContext.Object);



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

            Assert.True(((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200);

            Assert.True((int)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value > 0);
        }
    }
}
