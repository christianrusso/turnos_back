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

        public HairdressingEmployeeController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _businessPlaceServive = new BusinessPlaceService();
        }

        /// <summary>
        /// Agrega un empleado
        /// </summary>
        [HttpPost]
        public void Register([FromBody] RegisterEmployeeDto employeeDto)
        {
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
            }
        }

        /// <summary>
        /// Elimina un empleado
        /// </summary>
        [HttpPost]
        public void Remove([FromBody] RemoveEmployeeDto employeeDto)
        {
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
        }
    }
}
