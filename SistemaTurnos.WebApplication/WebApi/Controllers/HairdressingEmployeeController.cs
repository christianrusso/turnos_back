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
using SistemaTurnos.WebApplication.WebApi.Dto.Employee;
using SistemaTurnos.WebApplication.WebApi.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using SistemaTurnos.WebApplication.WebApi.Dto.Email;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    [Authorize(Roles = Roles.Administrator)]
    public class HairdressingEmployeeController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly BusinessPlaceService _businessPlaceServive;
        private readonly EmailService _emailService;

        public HairdressingEmployeeController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _businessPlaceServive = new BusinessPlaceService();
            _emailService = new EmailService();
        }

        /// <summary>
        /// Agrega un empleado
        /// </summary>
        [HttpPost]
        public void Register([FromBody] RegisterEmployeeDto employeeDto)
        {
            var watch = Stopwatch.StartNew();

            var emailMessage = new EmailDto();

            if (!_roleManager.RoleExistsAsync(Roles.Employee).Result)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }

            var userId = _businessPlaceServive.GetUserIdOrDefault(HttpContext);

            if (userId == null)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }

            var user = new ApplicationUser
            {
                UserName = employeeDto.Email,
                Email = employeeDto.Email
            };

            var result = _userManager.CreateAsync(user, employeeDto.Password).Result;

            if (!result.Succeeded)
            {
                throw new ApplicationException(ExceptionMessages.UsernameAlreadyExists);
            }

            using (var dbContext = new ApplicationDbContext())
            {
                var appUser = _userManager.Users.SingleOrDefault(au => au.Email == employeeDto.Email);

                result = _userManager.AddToRoleAsync(appUser, Roles.Employee).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                var employee = new Hairdressing_Employee
                {
                    UserId = appUser.Id,
                    OwnerUserId = userId.Value
                };

                dbContext.Hairdressing_Employees.Add(employee);
                dbContext.SaveChanges();

                emailMessage = new EmailDto
                {
                    From = "no-reply@todoreservas.com.ar",
                    Subject = "Empleado registrado",
                    To = new List<string> { appUser.Email },
                    Message = "Empleado registrado"
                };
            }

            _emailService.Send(emailMessage);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingEmployee/Register milisegundos: " + elapsedMs);
        }

        /// <summary>
        /// Elimina un empleado
        /// </summary>
        [HttpPost]
        public void Remove([FromBody] RemoveEmployeeDto employeeDto)
        {
            var watch = Stopwatch.StartNew();

            var appUser = _userManager.Users.SingleOrDefault(user => user.Email == employeeDto.Email);

            if (appUser == null)
            {
                throw new ApplicationException(ExceptionMessages.UserDoesNotExists);
            }

            var resultDelete = _userManager.DeleteAsync(appUser).Result;

            if (!resultDelete.Succeeded)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingEmployee/Remove milisegundos: " + elapsedMs);
        }

        /// <summary>
        /// Obtiene todos los empleados
        /// </summary>
        [HttpGet]
        public List<EmployeeDto> GetAll()
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var res = dbContext.Hairdressing_Employees
                    .Select(s => new EmployeeDto {
                        Email = s.User.Email
                    }).ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("HairdressingEmployee/GetAll milisegundos: " + elapsedMs);

                return res;
            }
        }
    }
}
