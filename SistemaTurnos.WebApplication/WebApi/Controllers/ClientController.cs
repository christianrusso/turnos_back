using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.Database.ClinicModel;
using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Client;
using SistemaTurnos.WebApplication.WebApi.Dto.Clinic;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class ClientController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;

        public ClientController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost]
        public void Register([FromBody] RegisterClientDto clientDto)
        {
            if (!_roleManager.RoleExistsAsync(Roles.Client).Result)
            {
                throw new ApplicationException(ExceptionMessages.RolesHaveNotBeenCreated);
            }

            var user = new ApplicationUser
            {
                UserName = clientDto.Email,
                Email = clientDto.Email
            };

            var result = _userManager.CreateAsync(user, clientDto.Password).Result;

            if (!result.Succeeded)
            {
                throw new ApplicationException(ExceptionMessages.UsernameAlreadyExists);
            }

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
                    UserId = appUser.Id,
                    Logo = ""                    
                };

                dbContext.Clinic_Clients.Add(client);
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public void Remove([FromBody] RemoveClientDto clientDto)
        {
            var appUser = _userManager.Users.SingleOrDefault(user => user.Email == clientDto.Email);

            if (appUser == null)
            {
                throw new ApplicationException(ExceptionMessages.UserDoesNotExists);
            }

            var resultDelete = _userManager.DeleteAsync(appUser).Result;

            if (!resultDelete.Succeeded)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
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

        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public void AddFavoriteClinic([FromBody] IdDto clinic)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                if (!dbContext.Clinics.Any(c => c.Id == clinic.Id))
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var client = dbContext.Clinic_Clients.FirstOrDefault(c => c.UserId == userId);

                if (client.FavoriteClinics.Any(fc => fc.ClinicId == clinic.Id))
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                client.FavoriteClinics.Add(new Clinic_ClientFavoriteClinics
                {
                    ClientId = client.Id,
                    ClinicId = clinic.Id
                });

                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public void RemoveFavoriteClinic([FromBody] IdDto clinic)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                if (!dbContext.Clinics.Any(c => c.Id == clinic.Id))
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var client = dbContext.Clinic_Clients.FirstOrDefault(c => c.UserId == userId);

                var favoriteClinic = client.FavoriteClinics.Where(fc => fc.ClinicId == clinic.Id);

                if (favoriteClinic == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                dbContext.Entry(favoriteClinic).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
        }

        [HttpGet]
        [Authorize(Roles = Roles.Client)]
        public List<ClinicDto> GetFavoriteClinics()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var client = dbContext.Clinic_Clients.FirstOrDefault(c => c.UserId == userId);

                return client.FavoriteClinics.Select(fv => new ClinicDto
                {
                    ClinicId = fv.ClinicId,
                    Name = fv.Clinic.Name,
                    Description = fv.Clinic.Description,
                    Address = fv.Clinic.Address,
                    City = fv.Clinic.City.Name,
                    Latitude = fv.Clinic.Latitude,
                    Longitude = fv.Clinic.Longitude,
                    Logo = fv.Clinic.Logo,
                    DistanceToUser = -1
                })
                .ToList();
            }
        }

        private int GetUserId()
        {
            int? userId = (int?)HttpContext.Items["userId"];

            if (!userId.HasValue)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }

            return userId.Value;
        }
    }
}
