using System;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Database;
using SistemaTurnos.Database.ModelData;
using SistemaTurnos.Database.HairdressingModel;

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
            context.Cities.Add(new City() { Id = 1, Name = "City" });



            //subspecialities
            context.Hairdressing_Subspecialties.Add(new Hairdressing_Subspecialty()
            {
                Id = 1,
                UserId = 1,
                Data = new SubspecialtyData()
                {
                    Id = 2,
                    Description = "desc",
                },
                Specialty = new Hairdressing_Specialty()
                {
                    Id = 1,
                    Data = new SpecialtyData()
                    {
                        Id = 1,
                        Description = ""
                    }
                },
                ConsultationLength = 2
            });

            context.Hairdressing_Subspecialties.Add(new Hairdressing_Subspecialty()
            {
                Id = 2,
                UserId = 1,
                Data = new SubspecialtyData()
                {
                    Id = 2,
                    Description = "desc2",
                },
                Specialty = new Hairdressing_Specialty()
                {
                    Id = 1,
                    Data = new SpecialtyData()
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
