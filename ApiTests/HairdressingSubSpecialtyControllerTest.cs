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
    public class HairdressingSubSpecialtyControllerTest
    {
        [Fact]
        public void GetAll_Ok()
        {
            var dbContext = DbContextFixture.GetDbContext();

            var httpContext = IdentityFixture.GetHttpContext();

            var controller = new HairdressingSubSpecialtyController(dbContext);

            

            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext();
            controller.ControllerContext.HttpContext = httpContext.Object;
            controller._service = new SistemaTurnos.WebApplication.WebApi.Services.BusinessPlaceService(httpContext.Object);

            var res = controller.GetAll();
            Assert.True(res.Count == 2);
        }
    }
}
