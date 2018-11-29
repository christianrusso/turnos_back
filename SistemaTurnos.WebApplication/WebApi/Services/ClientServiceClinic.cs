using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database;
using SistemaTurnos.Database.ClinicModel;
using SistemaTurnos.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Dto.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Services
{
    public class ClientServiceClinic : ClientServiceBase
    {
        public ClientServiceClinic(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
            : base(userManager, roleManager, signInManager, configuration) { }

        public void Register(RegisterClientDto clientDto)
        {
            var result = RegisterBase(clientDto);

            using (var dbContext = new ApplicationDbContext())
            {
                var appUser = _userManager.Users.SingleOrDefault(au => au.Email == clientDto.Email);

                result = _userManager.AddToRoleAsync(appUser, Roles.Client).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                var client = new SystemClient
                {
                    UserId = appUser.Id,
                    Logo = "",

                    FirstName = clientDto.FirstName,
                    LastName = clientDto.LastName,
                    Dni = clientDto.Dni,
                    Address = clientDto.Address,
                    PhoneNumber = clientDto.PhoneNumber
                };

                dbContext.Clients.Add(client);
                dbContext.SaveChanges();
            }
        }

        public List<ClientDto> GetAllNonPatientsByFilter(HttpContext httpContex, ClientFilterDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId(httpContex);

                return dbContext.Clients
                    .Where(c => !c.Patients.Any(p => p.UserId == userId))
                    .Where(c => filter.Dni == null || c.Dni.Contains(filter.Dni))
                    .Where(c => filter.Email == null || c.User.Email.Contains(filter.Email))
                    .Select(c => new ClientDto
                    {
                        Id = c.Id,
                        Email = c.User.Email,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Dni = c.Dni,
                        Address = c.Address,
                        PhoneNumber = c.PhoneNumber
                    }).ToList();
            }
        }

        internal void Edit(EditClientDto clientDto, HttpContext httpContext)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId(httpContext);

                var clientToUpdate = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                if (clientToUpdate == null)
                {
                    throw new BadRequestException();
                }

                clientToUpdate.FirstName = clientDto.FirstName;
                clientToUpdate.LastName = clientDto.LastName;
                clientToUpdate.Address = clientDto.Address;
                clientToUpdate.PhoneNumber = clientDto.PhoneNumber;
                clientToUpdate.Dni = clientDto.Dni;

                dbContext.SaveChanges();
            }
        }

        public List<ClientDto> GetAllNonHairdressingPatients(HttpContext httpContex, ClientFilterDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId(httpContex);

                return dbContext.Clients
                    .Where(c => !c.HairdressingPatients.Any(p => p.UserId == userId))
                    .Where(c => filter.Dni == null || c.Dni.Contains(filter.Dni))
                    .Where(c => filter.Email == null || c.User.Email.Contains(filter.Email))
                    .Select(c => new ClientDto
                    {
                        Id = c.Id,
                        Email = c.User.Email,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Dni = c.Dni,
                        Address = c.Address,
                        PhoneNumber = c.PhoneNumber
                    }).ToList();
            }
        }
    }
}
