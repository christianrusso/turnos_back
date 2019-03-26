using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Dto.Client;
using System;
using System.Linq;


namespace SistemaTurnos.WebApplication.WebApi.Services
{
    public class ClientServiceBase : ServiceBase
    {
        protected readonly SignInManager<ApplicationUser> _signInManager;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly RoleManager<ApplicationRole> _roleManager;
        protected readonly IConfiguration _configuration;
        
        
        public ClientServiceBase(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public void Remove( RemoveClientDto clientDto)
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

        
    }
}
