﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto.Account;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.Database;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost]
        public void CreateRoles()
        {
            CreateRole(Roles.Administrator);
            CreateRole(Roles.Employee);
            CreateRole(Roles.Client);
        }

        [HttpPost]
        public object Login([FromBody] LoginAccountDto model)
        {
            var result = _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false).Result;

            if (!result.Succeeded)
            {
                throw new ApplicationException(ExceptionMessages.LoginFailed);
            }

            var appUser = _userManager.Users.SingleOrDefault(user => user.Email == model.Email);
            string token = GenerateJwtToken(model.Email, appUser);
            int userId = appUser.Id;

            if (_userManager.IsInRoleAsync(appUser, Roles.Employee).Result)
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var employee = dbContext.Employees.FirstOrDefault(e => e.UserId == appUser.Id);
                    userId = employee.OwnerUserId;
                }
            }

            ValidTokens.Add($"{JwtBearerDefaults.AuthenticationScheme} {token}", userId);

            return Ok(new { token });
        }

        [HttpPost]
        public void Register([FromBody] RegisterAccountDto model)
        {
            if (!_roleManager.RoleExistsAsync(Roles.Administrator).Result)
            {
                throw new ApplicationException(ExceptionMessages.RolesHaveNotBeenCreated);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = _userManager.CreateAsync(user, model.Password).Result;
            
            if (!result.Succeeded)
            {
                throw new ApplicationException(ExceptionMessages.UsernameAlreadyExists);
            }

            using (var dbContext = new ApplicationDbContext())
            {
                var appUser = _userManager.Users.SingleOrDefault(au => au.Email == model.Email);

                result = _userManager.AddToRoleAsync(appUser, Roles.Administrator).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                var clinic = new Clinic
                {
                    Address = model.Address,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    UserId = appUser.Id
                };

                dbContext.Clinics.Add(clinic);
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        [Authorize]
        public void Remove([FromBody] RemoveAccountDto model)
        {
            var appUser = _userManager.Users.SingleOrDefault(user => user.Email == model.Email);

            if (appUser == null)
            {
                throw new ApplicationException(ExceptionMessages.UserDoesNotExists);
            }

            var resultPassword = _signInManager.CheckPasswordSignInAsync(appUser, model.Password, false).Result;

            if (!resultPassword.Succeeded)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }

            var resultDelete = _userManager.DeleteAsync(appUser).Result;

            if (!resultDelete.Succeeded)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        public void Edit([FromBody] EditAccountDto model)
        {
            var appUser = _userManager.Users.SingleOrDefault(user => user.Email == model.Email);

            if (appUser == null)
            {
                throw new ApplicationException(ExceptionMessages.UserDoesNotExists);
            }

            var resultPassword = _signInManager.CheckPasswordSignInAsync(appUser, model.OldPassword, false).Result;

            if (!resultPassword.Succeeded)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }

            var resultChangePassword = _userManager.ChangePasswordAsync(appUser, model.OldPassword, model.NewPassword).Result;

            if (!resultChangePassword.Succeeded)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public void Logout()
        {
            _signInManager.SignOutAsync().Wait();

            // Clear the existing external cookie to ensure a clean login process
            HttpContext.SignOutAsync(IdentityConstants.ExternalScheme).Wait();

            var isAuthorized = HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues token);

            if (!isAuthorized)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }

            ValidTokens.Remove(token);
        }

        private string GenerateJwtToken(string email, ApplicationUser user)
        {
            var roles = _userManager.GetRolesAsync(user).Result;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void CreateRole(string roleName)
        {
            bool exists = _roleManager.RoleExistsAsync(roleName).Result;

            if (!exists)
            {
                var role = new ApplicationRole
                {
                    Name = roleName
                };

                var result = _roleManager.CreateAsync(role).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }
            }
        }
    }
}
