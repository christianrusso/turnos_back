using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SistemaTurnos.Database;
using SistemaTurnos.Database.ClinicModel;
using SistemaTurnos.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto.Employee;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using System;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    [Authorize(Roles = Roles.Administrator)]
    public class EmployeeController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;

        public EmployeeController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Agrega un empleado
        /// </summary>
        [HttpPost]
        public void Register([FromBody] RegisterEmployeeDto employeeDto)
        {
            if (!_roleManager.RoleExistsAsync(Roles.Employee).Result)
            {
                throw new ApplicationException(ExceptionMessages.RolesHaveNotBeenCreated);
            }

            var userId = GetUserId();

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

                var employee = new Clinic_Employee
                {
                    UserId = appUser.Id,
                    OwnerUserId = userId
                };

                dbContext.Clinic_Employees.Add(employee);
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
