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
using SistemaTurnos.WebApplication.WebApi.Dto.Client;
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

        private readonly ClientServiceClinic _ClientService;

        public ClientController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;

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
            var clients = _ClientService.GetAllNonPatients();

            return clients;
        }

        
    }
}
