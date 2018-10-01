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
using SistemaTurnos.WebApplication.WebApi.Services;
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
        private BusinessPlaceService _service;

        private readonly ClientServiceClinic _ClientService;

        public ClientController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _service = new BusinessPlaceService(this.HttpContext);
            
            _ClientService = new ClientServiceClinic(_userManager, _roleManager, _signInManager, _configuration, HttpContext);
        }

        [HttpPost]
        public void Register([FromBody] RegisterClientDto clientDto)
        {
            _ClientService.Register(clientDto);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public void Remove([FromBody] RemoveClientDto clientDto)
        {
            _ClientService.Remove(clientDto);
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public List<ClientDto> GetAllNonPatients()
        {
            var clients = _ClientService.GetAllNonPatients(this.HttpContext);

            return clients;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public void AddFavoriteClinic([FromBody] IdDto clinic)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                if (!dbContext.Clinics.Any(c => c.Id == clinic.Id))
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                if (client.FavoriteClinics.Any(fc => fc.ClinicId == clinic.Id))
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                client.FavoriteClinics.Add(new Clinic_ClientFavorite
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
                var userId = _service.GetUserId(this.HttpContext);

                if (!dbContext.Clinics.Any(c => c.Id == clinic.Id))
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

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
                var userId = _service.GetUserId(this.HttpContext);

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

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
    }
}
