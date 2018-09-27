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
    public class DbContextFixture
    {
        public static ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                      .UseInMemoryDatabase(Guid.NewGuid().ToString())
                      .Options;
            var context = new ApplicationDbContext(options);
            context.Cities.Add(new SistemaTurnos.WebApplication.Database.ModelData.City() { Id = 1, Name = "City" });



            //subspecialities
            context.Hairdressing_Subspecialties.Add(new SistemaTurnos.WebApplication.Database.HairdressingModel.Hairdressing_Subspecialty()
            {
                Id = 1,
                UserId = 1,
                Data = new SistemaTurnos.WebApplication.Database.ModelData.SubspecialtyData()
                {
                    Id = 2,
                    Description = "desc",
                },
                Specialty = new SistemaTurnos.WebApplication.Database.HairdressingModel.Hairdressing_Specialty()
                {
                    Id = 1,
                    Data = new SistemaTurnos.WebApplication.Database.ModelData.SpecialtyData()
                    {
                        Id = 1,
                        Description = ""
                    }
                },
                ConsultationLength = 2
            });

            context.Hairdressing_Subspecialties.Add(new SistemaTurnos.WebApplication.Database.HairdressingModel.Hairdressing_Subspecialty()
            {
                Id = 2,
                UserId = 1,
                Data = new SistemaTurnos.WebApplication.Database.ModelData.SubspecialtyData()
                {
                    Id = 2,
                    Description = "desc2",
                },
                Specialty = new SistemaTurnos.WebApplication.Database.HairdressingModel.Hairdressing_Specialty()
                {
                    Id = 1,
                    Data = new SistemaTurnos.WebApplication.Database.ModelData.SpecialtyData()
                    {
                        Id = 1,
                        Description = ""
                    }
                },
                ConsultationLength = 2
            });



            context.SaveChanges();

            return context;
        }
    }
}
