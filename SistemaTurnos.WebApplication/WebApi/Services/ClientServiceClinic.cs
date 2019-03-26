﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database;
using SistemaTurnos.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Dto.Client;
using SistemaTurnos.WebApplication.WebApi.Dto.Email;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Services
{
    public class ClientServiceClinic : ClientServiceBase
    {
        private readonly EmailService _emailService = new EmailService();

        public ClientServiceClinic(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
            : base(userManager, roleManager, signInManager, configuration) { }

        public void Register(RegisterClientDto clientDto)
        {
            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {

                var appUser = _userManager.Users.SingleOrDefault(u => u.UserName == clientDto.Username);
                if (appUser != null)
                {
                    throw new ApplicationException(ExceptionMessages.UsernameAlreadyExists);
                }

                var result = RegisterBase(clientDto);

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
                    Address = clientDto.Address
                };

                dbContext.Clients.Add(client);
                dbContext.SaveChanges();

                string template = "<html lang='en'> <head> <meta charset='UTF-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta name='viewport' content='width=device-width, initial-scale=1'> <title>Mail cancelación</title> </head> <body> <table style='max-width: 600px; width:100%;height: 100vh;margin:auto;border-spacing: 0px;'> <thead> <tr style='height:65px;background-color: #373fc2;'> <th><img src='http://todoreservas.com.ar/panel/assets/img/logo.jpg' alt='Todo Reservas'></th> </tr> </thead> <tbody> <tr style='height: 167px;background-color: #454edb;display: block;'> <th style='width: 100%;display: block;'> <span style='font-family: Roboto;font-size: 25px;font-weight: 400;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #ffffff;display: block;padding-bottom:10px;padding-top: 25px;'>¡Felicitaciones!</span> <span style='font-family: Roboto;font-size: 16px;font-weight: 100;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #ffffff;display: block;'>Su registro se ha realizado con éxito</span> </th> <th style='display: block; margin: auto;'> <img src='http://todoreservas.com.ar/panel/assets/img/usercuadrado.png' height='111' width='111' alt='Ticket' style='padding-top: 20px;'> </th> </tr> <tr style='display: block;border-left: 1px solid #cccccc; border-right: 1px solid #cccccc;padding-bottom: 50px;'> <th style='width: 100%;display: block;padding-top: 115px;'> <span style='font-family: Roboto;font-size: 14px;font-weight: 600;font-style: normal;font-stretch: normal;line-height: 1.14;letter-spacing: normal;text-align: center;color: #060706;display: block;'>¡Ya puede acceder a su cuenta y comenzar a disfrutar!</span> <span style='font-family: Roboto;font-size: 14px; font-weight: 300; font-style: normal; font-stretch: normal; line-height: 1.14;letter-spacing: normal;text-align: center;color: #060706;display: block;padding-top:10px'></span> <span style='display: block;padding-top: 40px;'><a href='http://todoreservas.com.ar/' style='font-family: Roboto;font-size: 12px;font-weight: 500;font-style: normal;font-stretch: normal;line-height: 30px;letter-spacing: normal;text-align: center;color: #ffffff;height: 30px;border-radius: 15px;background-color: #00b900;display:inline-block;padding: 0px 10px;text-decoration: none;'>INICIAR SESIÓN</a></span> </th> </tr> <tr style='display: block; padding-top: 30px;padding-bottom: 30px;border: 1px solid #ccc;'> <th style='width:100%;text-align:center;display: block;'> <span style='font-family: Roboto;font-size: 12.5px;font-weight: 300;font-style: normal;font-stretch: normal;line-height: 1.17;letter-spacing: normal;text-align: center;color: #060706;padding-right: 10px;'>¿Tiene dudas?</span> <span style='border-radius: 13px;border: 1px solid #030303;padding:3px 10px;'><a href='http://todoreservas.com.ar/preguntasFrecuentes' style='font-family: Roboto;font-size: 11px;font-weight: 300;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #030303;text-decoration: none'>CENTRO DE AYUDA</a></span> </th> </tr> </tbody> </table> </body></html>";
                
                emailMessage = new EmailDto
                {
                    From = "no-reply@todoreservas.com.ar",
                    Subject = "Cliente registrado",
                    To = new List<string> { appUser.Email },
                    Message = template
                };
            }

            _emailService.Send(emailMessage);
        }

        public List<ClientDto> GetAllNonPatientsByFilter(HttpContext httpContex, ClientFilterDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId(httpContex);

                return dbContext.Clients
                    .Where(c => !c.Patients.Any(p => p.UserId == userId))
                    .Where(c => filter.PhoneNumber == null || c.PhoneNumber.Contains(filter.PhoneNumber))
                    .Where(c => filter.Email == null || c.User.Email.ToLower().Contains(filter.Email.ToLower()))
                    .Select(c => new ClientDto
                    {
                        Id = c.Id,
                        Email = c.User.Email,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
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
                    .Where(c => filter.PhoneNumber == null || c.PhoneNumber.Contains(filter.PhoneNumber))
                    .Where(c => filter.Email == null || c.User.Email.ToLower().Contains(filter.Email.ToLower()))
                    .Select(c => new ClientDto
                    {
                        Id = c.Id,
                        Email = c.User.Email,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Address = c.Address,
                        PhoneNumber = c.PhoneNumber
                    }).ToList();
            }
        }
    }
}
