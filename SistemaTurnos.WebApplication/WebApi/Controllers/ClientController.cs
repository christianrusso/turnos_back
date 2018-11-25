using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database;
using SistemaTurnos.Database.ClinicModel;
using SistemaTurnos.Database.HairdressingModel;
using SistemaTurnos.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Client;
using SistemaTurnos.WebApplication.WebApi.Dto.Clinic;
using SistemaTurnos.WebApplication.WebApi.Dto.Common;
using SistemaTurnos.WebApplication.WebApi.Dto.Hairdressing;
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

        /// <summary>
        /// Registra un cliente
        /// </summary>
        [HttpPost]
        public void Register([FromBody] RegisterClientDto clientDto)
        {
            _ClientService.Register(clientDto);
        }

        /// <summary>
        /// Edita un cliente
        /// </summary>
        [HttpPost]
        public void Edit([FromBody] EditClientDto clientDto)
        {
            _ClientService.Edit(clientDto, this.HttpContext);
        }

        /// <summary>
        /// Elimina cliente
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public void Remove([FromBody] RemoveClientDto clientDto)
        {
            _ClientService.Remove(clientDto);
        }

        /// <summary>
        /// Devuelve todos los clientes que no son pacientes
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public List<ClientDto> GetAllNonPatients()
        {
            return _ClientService.GetAllNonPatients(HttpContext);
        }

        /// <summary>
        /// Devuelve todos los clientes que no son pacientes de peluqueria
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public List<ClientDto> GetAllNonHairdressingPatients()
        {
            var clients = _ClientService.GetAllNonHairdressingPatients(this.HttpContext);

            return clients;
        }

        /// <summary>
        /// Agrega una clinica favorita
        /// </summary>
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

        /// <summary>
        /// Elimina una clinica favorita
        /// </summary>
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

                foreach (var f in favoriteClinic)
                {
                    dbContext.Clinic_ClientFavorites.Remove(f);
                }

                //dbContext.Entry(favoriteClinic).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene todas las clinicas favoritas
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Roles.Client)]
        public List<ClinicDto> GetFavoriteClinics()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                var res = new List<ClinicDto>();

                foreach (var fv in client.FavoriteClinics)
                {
                    var clinicUserId = fv.Clinic.UserId;
                    var ratings = dbContext.Clinic_Ratings.Where(r => r.UserId == clinicUserId).ToList();
                    var score = ratings.Any() ? ratings.Average(r => r.Score) : 0;

                    res.Add(new ClinicDto
                    {
                        ClinicId = fv.ClinicId,
                        Name = fv.Clinic.Name,
                        Description = fv.Clinic.Description,
                        Address = fv.Clinic.Address,
                        City = fv.Clinic.City.Name,
                        Latitude = fv.Clinic.Latitude,
                        Longitude = fv.Clinic.Longitude,
                        Logo = fv.Clinic.Logo,
                        DistanceToUser = -1,
                        Score = score
                    });
                }

                return res;
            }
        }

        /// <summary>
        /// Agrega una peluqueria favorita
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public void AddFavoriteHairdressing([FromBody] IdDto hairdressing)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                if (!dbContext.Hairdressings.Any(c => c.Id == hairdressing.Id))
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                if (client.FavoriteClinics.Any(fc => fc.ClinicId == hairdressing.Id))
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                client.FavoriteHairdressing.Add(new Hairdressing_ClientFavorite
                {
                    ClientId = client.Id,
                    HairdressingId = hairdressing.Id
                });

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina una peluqueria favorita
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public void RemoveFavoriteHairdressing([FromBody] IdDto hairdressing)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                if (!dbContext.Clinics.Any(c => c.Id == hairdressing.Id))
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                var favoriteHairdressing = client.FavoriteHairdressing.Where(fc => fc.HairdressingId == hairdressing.Id);

                if (favoriteHairdressing == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                foreach (var f in favoriteHairdressing)
                {
                    dbContext.Hairdressing_ClientFavorites.Remove(f);
                }
                
                //dbContext.Entry(favoriteHairdressing).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene todas las peluquerias favoritas
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Roles.Client)]
        public List<HairdressingDto> GetFavoriteHairdressing()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                return client.FavoriteHairdressing.Select(fv => new HairdressingDto
                {
                    HairdressingId = fv.HairdressingId,
                    Name = fv.Hairdressing.Name,
                    Description = fv.Hairdressing.Description,
                    Address = fv.Hairdressing.Address,
                    City = fv.Hairdressing.City.Name,
                    Latitude = fv.Hairdressing.Latitude,
                    Longitude = fv.Hairdressing.Longitude,
                    Logo = fv.Hairdressing.Logo,
                    DistanceToUser = -1
                })
                .ToList();
            }
        }

        /// <summary>
        /// Obtien todos los favoritos.
        /// </summary>
        //All favorites
        [HttpGet]
        [Authorize(Roles = Roles.Client)]
        public FavoritesDto GetFavorites()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                if (client == null)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                var hairdressingFavorites = client.FavoriteHairdressing.Select(fv => new HairdressingDto
                {
                    HairdressingId = fv.HairdressingId,
                    Name = fv.Hairdressing.Name,
                    Description = fv.Hairdressing.Description,
                    Address = fv.Hairdressing.Address,
                    City = fv.Hairdressing.City.Name,
                    Latitude = fv.Hairdressing.Latitude,
                    Longitude = fv.Hairdressing.Longitude,
                    Logo = fv.Hairdressing.Logo,
                    DistanceToUser = -1
                });

                var clinicFavorites = client.FavoriteClinics.Select(fv => new ClinicDto
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
                });

                var favoriteDto = new FavoritesDto();
                favoriteDto.HairdressingFavorites = hairdressingFavorites.ToList();
                favoriteDto.ClinicFavorites = clinicFavorites.ToList();

                return favoriteDto;
            }
        }

        /// <summary>
        /// Obtiene todos los turnos, de todos los rubros juntos, de un cliente dado entre un rango de fechas dado.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public WeekForClientDto GetWeekForClient([FromBody] FilterClientWeekDto filter)
        {
            var service = new AppointmentService(HttpContext);
            return service.GetWeekForClient(filter, HttpContext);
        }

        /// <summary>
        /// Obtiene los datos de perfil del cliente
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public ClientProfileDto GetProfile()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                if (client == null)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                return new ClientProfileDto
                {
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Address = client.Address,
                    PhoneNumber = client.PhoneNumber,
                    Dni = client.Dni,
                    Logo = client.Logo
                };
            }
        }
    }
}
