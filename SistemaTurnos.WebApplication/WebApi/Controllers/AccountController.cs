using System;
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
using SistemaTurnos.Database.Model;
using SistemaTurnos.Database;
using SistemaTurnos.Database.ClinicModel;
using SistemaTurnos.Database.HairdressingModel;
using SistemaTurnos.Database.Enums;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.WebApplication.WebApi.Services;
using SistemaTurnos.WebApplication.WebApi.Dto.Common;
using System.Diagnostics;
using SistemaTurnos.WebApplication.WebApi.Dto.Email;

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
        private readonly EmailService _emailService;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _dbContext = dbContext;
            _emailService = new EmailService();
        }

        /// <summary>
        /// Loging de una empresa.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public LogOnDto Login([FromBody] LoginAccountDto model)
        {
            var watch = Stopwatch.StartNew();

            var result = _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false).Result;

            if (!result.Succeeded)
            {
                throw new BadRequestException(ExceptionMessages.LoginFailed);
            }

            var appUser = _userManager.Users.SingleOrDefault(user => user.UserName == model.Username);

            if (appUser.FacebookLogin)
            {
                throw new BadRequestException(ExceptionMessages.LoginFailed);
            }

            int userId = appUser.Id;
            string logo = string.Empty;

            using (var dbContext = _dbContext)
            {
                if (_userManager.IsInRoleAsync(appUser, Roles.Administrator).Result)
                {
                    if (model.BusinessType.IsClinic() && !dbContext.Clinics.Any(c => c.UserId == appUser.Id))
                    {
                        throw new ApplicationException(ExceptionMessages.LoginFailed);
                    }

                    if (model.BusinessType.IsHBE() && !dbContext.Hairdressings.Any(h => h.BusinessType == model.BusinessType && h.UserId == appUser.Id))
                    {
                        throw new ApplicationException(ExceptionMessages.LoginFailed);
                    }
                }

                // El usuario es cliente administrador (clinica o peluqueria)
                if (_userManager.IsInRoleAsync(appUser, Roles.Administrator).Result)
                {
                    if (model.BusinessType == 0)
                    {
                        throw new BadRequestException();
                    }
                    if (model.BusinessType.IsClinic())
                    {
                        var clinic = dbContext.Clinics.FirstOrDefault(c => c.UserId == userId);

                        if (clinic == null)
                        {
                            throw new ApplicationException(ExceptionMessages.LoginFailed);
                        }

                        logo = clinic.Logo;
                    }
                    else if (model.BusinessType.IsHBE())
                    {
                        var Hemployee = dbContext.Hairdressing_Employees.FirstOrDefault(e => e.UserId == appUser.Id);
                        if (Hemployee != null)
                        {
                            userId = Hemployee.OwnerUserId;
                        }
                        var hairdressing = dbContext.Hairdressings.FirstOrDefault(h => h.BusinessType == model.BusinessType && h.UserId == userId);

                        if (hairdressing == null)
                        {
                            throw new ApplicationException(ExceptionMessages.LoginFailed);
                        }

                        logo = hairdressing.Logo;
                    }
                } // El usuario es un empleado
                else if (_userManager.IsInRoleAsync(appUser, Roles.Employee).Result)
                {
                    if (model.BusinessType == 0)
                    {
                        throw new BadRequestException();
                    }
                    if (model.BusinessType.IsClinic())
                    {
                        var employee = dbContext.Clinic_Employees.FirstOrDefault(e => e.UserId == appUser.Id);
                        if (employee != null)
                        {
                            userId = employee.OwnerUserId;
                        }
                        var clinic = dbContext.Clinics.FirstOrDefault(c => c.UserId == userId);

                        if (clinic == null)
                        {
                            throw new ApplicationException(ExceptionMessages.LoginFailed);
                        }

                        logo = clinic.Logo;
                    }
                    else if (model.BusinessType.IsHBE())
                    {
                        var Hemployee = dbContext.Hairdressing_Employees.FirstOrDefault(e => e.UserId == appUser.Id);
                        if (Hemployee != null)
                        {
                            userId = Hemployee.OwnerUserId;
                        }
                        var hairdressing = dbContext.Hairdressings.FirstOrDefault(h => h.BusinessType == model.BusinessType && h.UserId == userId);

                        if (hairdressing == null)
                        {
                            throw new ApplicationException(ExceptionMessages.LoginFailed);
                        }

                        logo = hairdressing.Logo;
                    }
                } // Es un cliente
                else if (_userManager.IsInRoleAsync(appUser, Roles.Client).Result)
                {
                    var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                    if (client == null)
                    {
                        throw new ApplicationException(ExceptionMessages.LoginFailed);
                    }

                    logo = client.Logo;
                }
            }

            string token = GenerateJwtToken(model.Username, appUser);
            ValidTokens.Add($"{JwtBearerDefaults.AuthenticationScheme} {token}", userId);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("AccountController/Login milisegundos: " + elapsedMs);

            return new LogOnDto
            {
                Token = token,
                Logo = logo,
                UserId = appUser.Id
            };
        }

        /// <summary>
        /// Loging de una empresa mediante Facebook, crea el usuario si es la primera vez que entra, sino chequea users ids.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public LogOnDto LoginFacebook([FromBody] LoginFacebookDto model)
        {
            var watch = Stopwatch.StartNew();

            if (FacebookService.GetUserId(model.Token) != model.UserId)
            {
                throw new ApplicationException(ExceptionMessages.LoginFailed);
            }

            using (var dbContext = new ApplicationDbContext())
            {
                var appUser = _userManager.Users.SingleOrDefault(u => u.UserName == model.UserId);
                if (appUser == null)
                {
                    if (!_roleManager.RoleExistsAsync(Roles.Client).Result)
                    {
                        throw new ApplicationException(ExceptionMessages.InternalServerError);
                    }

                    appUser = new ApplicationUser
                    {
                        UserName = model.UserId,
                        Email = model.Email,
                        PhoneNumber = "11111111",
                        FacebookLogin = true,
                    };

                    var result = _userManager.CreateAsync(appUser, model.UserId).Result;

                    if (!result.Succeeded)
                    {
                        throw new ApplicationException(ExceptionMessages.UsernameAlreadyExists);
                    }

                    result = _userManager.AddToRoleAsync(appUser, Roles.Client).Result;

                    if (!result.Succeeded)
                    {
                        throw new ApplicationException(ExceptionMessages.InternalServerError);
                    }

                    // TODO: REQUEST FACEBOOK IMAGE.

                    var client = new SystemClient
                    {
                        UserId = appUser.Id,
                        Logo = "",
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Address = ""
                    };

                    dbContext.Clients.Add(client);
                    dbContext.SaveChanges();
                }

                string token = GenerateJwtToken(model.UserId, appUser);
                ValidTokens.Add($"{JwtBearerDefaults.AuthenticationScheme} {token}", appUser.Id);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("AccountController/LoginFacebook milisegundos: " + elapsedMs);

                return new LogOnDto
                {
                    Token = token,
                    Logo = "",
                    UserId = appUser.Id
                };
            }
        }

        /// <summary>
        /// Registro de empresa. Hace falta seleccionar que tipo de rubro tiene.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register([FromBody] RegisterAccountDto model)
        {
            var watch = Stopwatch.StartNew();

            var emailMessage = new EmailDto();

            if (!_roleManager.RoleExistsAsync(Roles.Administrator).Result)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
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
                    throw new BadRequestException();
                }

                if (model.BusinessType == 0)
                {
                    throw new BadRequestException();
                }

                if (model.BusinessType.IsClinic())
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
                if (model.BusinessType.IsHBE())
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
                        Logo = model.Logo,
                        RequiresPayment = false,
                        BusinessType = model.BusinessType
                    };

                    dbContext.Hairdressings.Add(hairdressing);
                }

                dbContext.SaveChanges();
            }

            string template = "<html lang='en'> <head> <meta charset='UTF-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta name='viewport' content='width=device-width, initial-scale=1'> <title>Mail cancelación</title> </head> <body> <table style='max-width: 600px; width:100%;height: 100vh;margin:auto;border-spacing: 0px;'> <thead> <tr style='height:65px;background-color: #373fc2;'> <th><img src='http://todoreservas.com.ar/panel/assets/img/logo.jpg' alt='Todo Reservas'></th> </tr> </thead> <tbody> <tr style='height: 167px;background-color: #454edb;display: block;'> <th style='width: 100%;display: block;'> <span style='font-family: Roboto;font-size: 25px;font-weight: 400;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #ffffff;display: block;padding-bottom:10px;padding-top: 25px;'>¡Felicitaciones!</span> <span style='font-family: Roboto;font-size: 16px;font-weight: 100;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #ffffff;display: block;'>Su registro se ha realizado con éxito</span> </th> <th style='display: block; margin: auto;'> <img src='http://todoreservas.com.ar/panel/assets/img/usercuadrado.png' height='111' width='111' alt='Ticket' style='padding-top: 20px;'> </th> </tr> <tr style='display: block;border-left: 1px solid #cccccc; border-right: 1px solid #cccccc;padding-bottom: 50px;'> <th style='width: 100%;display: block;padding-top: 115px;'> <span style='font-family: Roboto;font-size: 14px;font-weight: 600;font-style: normal;font-stretch: normal;line-height: 1.14;letter-spacing: normal;text-align: center;color: #060706;display: block;'>¡Ya puede acceder a su cuenta y comenzar a disfrutar!</span> <span style='font-family: Roboto;font-size: 14px; font-weight: 300; font-style: normal; font-stretch: normal; line-height: 1.14;letter-spacing: normal;text-align: center;color: #060706;display: block;padding-top:10px'></span> <span style='display: block;padding-top: 40px;'><a href='http://todoreservas.com.ar/' style='font-family: Roboto;font-size: 12px;font-weight: 500;font-style: normal;font-stretch: normal;line-height: 30px;letter-spacing: normal;text-align: center;color: #ffffff;height: 30px;border-radius: 15px;background-color: #00b900;display:inline-block;padding: 0px 10px;text-decoration: none;'>INICIAR SESIÓN</a></span> </th> </tr> <tr style='display: block; padding-top: 30px;padding-bottom: 30px;border: 1px solid #ccc;'> <th style='width:100%;text-align:center;display: block;'> <span style='font-family: Roboto;font-size: 12.5px;font-weight: 300;font-style: normal;font-stretch: normal;line-height: 1.17;letter-spacing: normal;text-align: center;color: #060706;padding-right: 10px;'>¿Tiene dudas?</span> <span style='border-radius: 13px;border: 1px solid #030303;padding:3px 10px;'><a href='http://todoreservas.com.ar/preguntasFrecuentes' style='font-family: Roboto;font-size: 11px;font-weight: 300;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #030303;text-decoration: none'>CENTRO DE AYUDA</a></span> </th> </tr> </tbody> </table> </body></html>";
            
            emailMessage = new EmailDto
            {
                From = "no-reply@todoreservas.com.ar",
                Subject = "Usuario registrado",
                To = new List<string> { user.Email },
                Message = template
            };

            _emailService.Send(emailMessage);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("AccountController/Register milisegundos: " + elapsedMs);

            return Ok(model.BusinessType);
        }

        /// <summary>
        /// Eliminar cuenta
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Editar cuenta. Necesita estar logueado.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Logout
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public void Logout()
        {
            var watch = Stopwatch.StartNew();

            _signInManager.SignOutAsync().Wait();

            // Clear the existing external cookie to ensure a clean login process
            HttpContext.SignOutAsync(IdentityConstants.ExternalScheme).Wait();

            var isAuthorized = HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues token);

            if (!isAuthorized)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }

            ValidTokens.Remove(token);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("AccountController/Logout milisegundos: " + elapsedMs);
        }

        [HttpGet]
        [Authorize]
        public ProfileDto GetProfile()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = new BusinessPlaceService().GetUserId(HttpContext);

                var clinic = dbContext.Clinics.FirstOrDefault(c => c.UserId == userId);

                if (clinic != null)
                {
                    return new ProfileDto
                    {
                        Name = clinic.Name,
                        Address = clinic.Address,
                        Description = clinic.Description,
                        City = clinic.City.Name,
                        Latitude = clinic.Latitude,
                        Longitude = clinic.Longitude,
                        Logo = clinic.Logo,
                        OpenCloseHours = clinic.OpenCloseHours.Select(och => new OpenCloseHoursDto { DayNumber = och.DayNumber, Start = och.Start, End = och.End }).ToList(),
                        Images = clinic.Images.Select(i => i.Data).ToList(),
                        Require = false,
                        ClientId = string.Empty,
                        ClientSecret = string.Empty
                    };
                }

                var hairdressing = dbContext.Hairdressings.FirstOrDefault(h => h.UserId == userId);

                if (hairdressing != null)
                {
                    return new ProfileDto
                    {
                        Name = hairdressing.Name,
                        Address = hairdressing.Address,
                        Description = hairdressing.Description,
                        City = hairdressing.City.Name,
                        Latitude = hairdressing.Latitude,
                        Longitude = hairdressing.Longitude,
                        Logo = hairdressing.Logo,
                        OpenCloseHours = hairdressing.OpenCloseHours.Select(och => new OpenCloseHoursDto { DayNumber = och.DayNumber, Start = och.Start, End = och.End }).ToList(),
                        Images = hairdressing.Images.Select(i => i.Data).ToList(),
                        Require = hairdressing.RequiresPayment,
                        ClientId = hairdressing.ClientId,
                        ClientSecret = hairdressing.ClientSecret
                    };
                }

                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }
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
    }
}
