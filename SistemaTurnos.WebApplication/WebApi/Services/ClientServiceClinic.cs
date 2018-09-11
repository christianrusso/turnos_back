using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.Database.ClinicModel;
using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto.Client;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Services
{
    public class ClientServiceClinic: ClientServiceBase
    {
        public ClientServiceClinic(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, HttpContext httpContext)
            : base(userManager, roleManager, signInManager, configuration, httpContext) { }

        public void Register(RegisterClientDto clientDto)
        {
            var result = base.RegisterBase(clientDto);

            using (var dbContext = new ApplicationDbContext())
            {
                var appUser = _userManager.Users.SingleOrDefault(au => au.Email == clientDto.Email);

                result = _userManager.AddToRoleAsync(appUser, Roles.Client).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                var client = new Clinic_Client
                {
                    UserId = appUser.Id
                };

                dbContext.Clinic_Clients.Add(client);
                dbContext.SaveChanges();
            }
        }

        public List<ClientDto> GetAllNonPatients()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Clinic_Clients
                    .Where(c => !c.Patients.Any(p => p.UserId == userId))
                    .ToList()
                    .Select(c => new ClientDto
                    {
                        Id = c.Id,
                        Email = c.User.Email
                    }).ToList();
            }
        }
    }
}
