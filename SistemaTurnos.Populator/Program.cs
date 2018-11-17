using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SistemaTurnos.Database;
using SistemaTurnos.Database.Model;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace SistemaTurnos.Populator
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            // Add our DbContext
            services.AddDbContext<ApplicationDbContext>(ServiceLifetime.Transient);

            // Add Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Identity settings
            services.Configure<IdentityOptions>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredUniqueChars = 1;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    options.Lockout.AllowedForNewUsers = true;

                    // User settings
                    options.User.RequireUniqueEmail = true;
                }
            );

            // Remove default claims
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // Add populator service
            services.AddScoped<IPopulator,Populator>();

            // Build the IoC from the service collection
            var provider = services.BuildServiceProvider();

            // Populator service
            var populator = provider.GetService<IPopulator>();
            populator.Populate();

            Console.WriteLine("Presione una tecla para finalizar...");
            Console.ReadKey();
        }
    }
}