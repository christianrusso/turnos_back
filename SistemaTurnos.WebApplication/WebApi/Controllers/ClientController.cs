using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database;
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
using System.Diagnostics;
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
            _service = new BusinessPlaceService();
            
            _ClientService = new ClientServiceClinic(_userManager, _roleManager, _signInManager, _configuration);
        }

        /// <summary>
        /// Registra un cliente
        /// </summary>
        [HttpPost]
        public void Register([FromBody] RegisterClientDto clientDto)
        {
            var watch = Stopwatch.StartNew();

            _ClientService.Register(clientDto);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("ClientController/Register milisegundos: " + elapsedMs);
        }

        /// <summary>
        /// Edita un cliente
        /// </summary>
        [HttpPost]
        public void Edit([FromBody] EditClientDto clientDto)
        {
            var watch = Stopwatch.StartNew();

            _ClientService.Edit(clientDto, HttpContext);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("ClientController/Edit milisegundos: " + elapsedMs);
        }

        /// <summary>
        /// Elimina cliente
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public void Remove([FromBody] RemoveClientDto clientDto)
        {
            var watch = Stopwatch.StartNew();

            _ClientService.Remove(clientDto);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("ClientController/Remove milisegundos: " + elapsedMs);
        }

        /// <summary>
        /// Devuelve todos los clientes que no son pacientes
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public List<ClientDto> GetAllNonPatients()
        {
            var watch = Stopwatch.StartNew();

            var res = _ClientService.GetAllNonPatientsByFilter(HttpContext, new ClientFilterDto());

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("ClientController/GetAllNonPatients milisegundos: " + elapsedMs);

            return res;
        }

        /// <summary>
        /// Devuelve todos los clientes que no son pacientes aplicando los filtros indicados por parametro
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public List<ClientDto> GetAllNonPatientsByFilter([FromBody] ClientFilterDto filter)
        {
            var watch = Stopwatch.StartNew();

            var res = _ClientService.GetAllNonPatientsByFilter(HttpContext, filter);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("ClientController/GetAllNonPatientsByFilter milisegundos: " + elapsedMs);

            return res;
        }

        /// <summary>
        /// Devuelve todos los clientes que no son pacientes de peluqueria
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public List<ClientDto> GetAllNonHairdressingPatients()
        {
            var watch = Stopwatch.StartNew();

            var res = _ClientService.GetAllNonHairdressingPatients(HttpContext, new ClientFilterDto());

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("ClientController/GetAllNonHairdressingPatients milisegundos: " + elapsedMs);

            return res;
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public List<ClientDto> GetAllNonHairdressingPatientsByFilter([FromBody] ClientFilterDto filter)
        {
            var watch = Stopwatch.StartNew();

            var res = _ClientService.GetAllNonHairdressingPatients(HttpContext, filter);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("ClientController/GetAllNonHairdressingPatientsByFilter milisegundos: " + elapsedMs);

            return res;
        }

        /// <summary>
        /// Agrega una peluqueria favorita
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public void AddFavoriteHairdressing([FromBody] IdDto hairdressing)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                if (!dbContext.Hairdressings.Any(c => c.Id == hairdressing.Id))
                {
                    throw new BadRequestException();
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                if (client.FavoriteHairdressing.Any(fc => fc.HairdressingId == hairdressing.Id))
                {
                    throw new BadRequestException();
                }

                client.FavoriteHairdressing.Add(new Hairdressing_ClientFavorite
                {
                    ClientId = client.Id,
                    HairdressingId = hairdressing.Id
                });

                dbContext.SaveChanges();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("ClientController/AddFavoriteHairdressing milisegundos: " + elapsedMs);
            }
        }

        /// <summary>
        /// Elimina una peluqueria favorita
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public void RemoveFavoriteHairdressing([FromBody] IdDto hairdressing)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                if (!dbContext.Hairdressings.Any(c => c.Id == hairdressing.Id))
                {
                    throw new BadRequestException();
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                var favoriteHairdressing = client.FavoriteHairdressing.Where(fc => fc.HairdressingId == hairdressing.Id);

                if (favoriteHairdressing == null)
                {
                    throw new BadRequestException();
                }

                foreach (var f in favoriteHairdressing)
                {
                    dbContext.Hairdressing_ClientFavorites.Remove(f);
                }
                
                dbContext.SaveChanges();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("ClientController/RemoveFavoriteHairdressing milisegundos: " + elapsedMs);
            }
        }

        /// <summary>
        /// Obtiene todas las peluquerias favoritas
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Roles.Client)]
        public List<HairdressingDto> GetFavoriteHairdressing()
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                var res = client.FavoriteHairdressing.Select(fv => new HairdressingDto
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

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("ClientController/GetFavoriteHairdressing milisegundos: " + elapsedMs);

                return res;
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
            var watch = Stopwatch.StartNew();

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
                    DistanceToUser = -1,
                    BusinessTypeId = fv.Hairdressing.BusinessTypeId,
                    Score = dbContext.Hairdressing_Ratings.Where(r => r.Appointment.Patient.UserId == fv.Hairdressing.UserId).Select(r => r.Score).ToList().DefaultIfEmpty<uint>(0).Average(r => r)
                });

                var favoriteDto = new FavoritesDto();
                favoriteDto.HairdressingFavorites = hairdressingFavorites.ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("ClientController/GetFavorites milisegundos: " + elapsedMs);

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
            var watch = Stopwatch.StartNew();

            var res = new AppointmentService().GetWeekForClient(filter, HttpContext);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("ClientController/GetWeekForClient milisegundos: " + elapsedMs);

            return res;
        }

        /// <summary>
        /// Obtiene los datos de perfil del cliente
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Roles.Client)]
        public ClientProfileDto GetProfile()
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                if (client == null)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                var res = new ClientProfileDto
                {
                    Username = client.User.UserName,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Email = client.User.Email,
                    Address = client.Address,
                    PhoneNumber = client.PhoneNumber,
                    Logo = client.Logo
                };

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("ClientController/GetProfile milisegundos: " + elapsedMs);

                return res;
            }
        }
    }
}
