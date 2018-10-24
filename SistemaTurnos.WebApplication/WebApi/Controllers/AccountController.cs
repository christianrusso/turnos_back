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
using SistemaTurnos.WebApplication.Database.ClinicModel;
using SistemaTurnos.WebApplication.Database.HairdressingModel;

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
        private readonly ApplicationDbContext _dbContext;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        [HttpPost]
        public void CreateRoles()
        {
            CreateRole(Roles.Administrator);
            CreateRole(Roles.Employee);
            CreateRole(Roles.Client);
        }

        /// <summary>
        /// dasdasdasd
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public LogOnDto Login([FromBody] LoginAccountDto model)
        {
            var result = _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false).Result;

            if (!result.Succeeded)
            {
                throw new ApplicationException(ExceptionMessages.LoginFailed);
            }

            var appUser = _userManager.Users.SingleOrDefault(user => user.Email == model.Email);
            string token = GenerateJwtToken(model.Email, appUser);
            int userId = appUser.Id;
            string logo = string.Empty;

            using (var dbContext = _dbContext)
            {
                if (!_userManager.IsInRoleAsync(appUser, Roles.Client).Result)
                {
                    // Es una clinica o un empleado de la clinica
                    if (_userManager.IsInRoleAsync(appUser, Roles.Employee).Result)
                    {
                        if(model.BusinessType == 0)
                        {
                            throw new ApplicationException(ExceptionMessages.BadRequest);
                        }

                        if(model.BusinessType == Database.Enums.BusinessType.Clinic)
                        {
                            var employee = dbContext.Clinic_Employees.FirstOrDefault(e => e.UserId == appUser.Id);
                            if (employee != null)
                            {
                                userId = employee.OwnerUserId;
                            }
                            var clinic = dbContext.Clinics.FirstOrDefault(c => c.UserId == userId);
                            logo = clinic.Logo;
                        }
                        
                        else if(model.BusinessType == Database.Enums.BusinessType.Hairdressing)
                        {
                            var Hemployee = dbContext.Hairdressing_Employees.FirstOrDefault(e => e.UserId == appUser.Id);
                            if (Hemployee != null)
                            {
                                userId = Hemployee.OwnerUserId;
                            }
                            var hairdressing = dbContext.Hairdressings.FirstOrDefault(c => c.UserId == userId);
                            logo = hairdressing.Logo;
                        }

                        
                    }
                }
                else if (_userManager.IsInRoleAsync(appUser, Roles.Client).Result)
                    {
                            var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);
                            logo = client.Logo;
                    }
                
            }
                
            ValidTokens.Add($"{JwtBearerDefaults.AuthenticationScheme} {token}", userId);

            return new LogOnDto
            {
                Token = token,
                Logo = logo
            };
        }

        [HttpPost]
        public LogOnDto LoginFacebook([FromBody] LoginFacebookDto model)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var client = dbContext.Clients.FirstOrDefault(c => c.User.Email == model.Email);

                // Si client es null, el usuario no esta registrado. Si es distinto de null, ya esta registrado.
                if (client == null)
                {
                    // Registrar cliente
                    if (!_roleManager.RoleExistsAsync(Roles.Client).Result)
                    {
                        throw new ApplicationException(ExceptionMessages.RolesHaveNotBeenCreated);
                    }

                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email
                    };

                    var result = _userManager.CreateAsync(user, Guid.NewGuid().ToString()).Result;

                    if (!result.Succeeded)
                    {
                        throw new ApplicationException(ExceptionMessages.UsernameAlreadyExists);
                    }

                    var applicationUser = _userManager.Users.SingleOrDefault(au => au.Email == model.Email);

                    result = _userManager.AddToRoleAsync(applicationUser, Roles.Client).Result;

                    if (!result.Succeeded)
                    {
                        throw new ApplicationException(ExceptionMessages.InternalServerError);
                    }

                    client = new SystemClient
                    {
                        UserId = applicationUser.Id,
                        Logo = "",
                        FacebookUserId = model.UserId
                    };

                    dbContext.Clients.Add(client);
                    dbContext.SaveChanges();
                }

                // Chequeo que el FacebookUserId sea correcto
                if (client.FacebookUserId != model.UserId)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                // Logueo al usuario
                var appUser = _userManager.Users.SingleOrDefault(user => user.Email == model.Email);
                string token = GenerateJwtToken(model.Email, appUser);
                int userId = appUser.Id;

                if (!_userManager.IsInRoleAsync(appUser, Roles.Client).Result)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                ValidTokens.Add($"{JwtBearerDefaults.AuthenticationScheme} {token}", userId);

                return new LogOnDto
                {
                    Token = token,
                    Logo = client.Logo
                };
            }
        }

        [HttpPost]
        public ActionResult Register([FromBody] RegisterAccountDto model)
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

            using (var dbContext = _dbContext)
            {
                var appUser = _userManager.Users.SingleOrDefault(au => au.Email == model.Email);

                result = _userManager.AddToRoleAsync(appUser, Roles.Administrator).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                if (string.IsNullOrWhiteSpace(model.Logo))
                {
                    model.Logo = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAFoAAAAoBAMAAACMbPD7AAAAG1BMVEXMzMyWlpbFxcWjo6OqqqqxsbGcnJy+vr63t7eN+fR5AAAACXBIWXMAAA7EAAAOxAGVKw4bAAAApElEQVQ4je2QsQrCQBBEJ5fLpt2AHxCJWCc2WkZFsTwx9kcQ0ypK6lR+t3eInWw6q3vVLrwdlgECgcAvVFXqy3dm7GvR1ubczMxnjjnZ7ESbclqmJZK6B54c3x6iHYGsslBdByyYMBft2BwZDLxcvuHIXUuoatu6bEwHFGDK5ewUhf8bJ4t7lhUjf9Nw8J2oduWW0U7Sq9ETX2Tvbaxr0Q4E/s8bo1sUV4qjWrAAAAAASUVORK5CYII=";
                }

                if (!dbContext.Cities.Any(c => c.Id == model.City))
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                if(model.BusinessType == 0)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                if(model.BusinessType == Database.Enums.BusinessType.Clinic)
                {
                    var clinic = new Clinic
                    {
                        Name = model.Name,
                        Description = model.Description,
                        CityId = model.City,
                        Address = model.Address,
                        Latitude = model.Latitude,
                        Longitude = model.Longitude,
                        UserId = appUser.Id,
                        Logo = model.Logo
                    };

                    dbContext.Clinics.Add(clinic);
                }
                if(model.BusinessType == Database.Enums.BusinessType.Hairdressing)
                {
                    var hairdressing = new Hairdressing
                    {
                        Name = model.Name,
                        Description = model.Description,
                        CityId = model.City,
                        Address = model.Address,
                        Latitude = model.Latitude,
                        Longitude = model.Longitude,
                        UserId = appUser.Id,
                        Logo = model.Logo
                    };

                    dbContext.Hairdressings.Add(hairdressing);
                }
                
                dbContext.SaveChanges();
            }

            return Ok(model.BusinessType);
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
