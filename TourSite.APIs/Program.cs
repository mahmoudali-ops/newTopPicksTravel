
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourSite.APIs.Errors;
using TourSite.APIs.Helper;
using TourSite.APIs.MidleWare;
using TourSite.Core;
using TourSite.Core.Mapping;
using TourSite.Core.Servicies.Contract;
using TourSite.Repository.Data;
using TourSite.Repository.Data.Contexts;
using TourSite.Repository.Repositories;
using TourSite.Service.Services.CatTours;
using TourSite.Service.Services.Destnations;
using TourSite.Service.Services.Emails;
using TourSite.Service.Services.TourImgs;
using TourSite.Service.Services.Tours;
using TourSite.Service.Services.Trasnfers;
using TourSite.Service.Services.Users;

namespace TourSite.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            var allowedCors = builder.Configuration.GetSection("AllowedCors").Get<string[]>() ?? Array.Empty<string>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy =>
                    {
                        policy.WithOrigins(allowedCors)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ===== DI =====
            builder.Services.AddDependency(builder.Configuration);

            var app = builder.Build();

            // ===== Middlewares Order (??? ????) =====
            app.UseRouting();

            app.UseCors("AllowAngular");

            app.UseAuthentication();   // ? ???? ??? Authorization
            app.UseAuthorization();    // ?

            app.UseSwagger();
            app.UseSwaggerUI();

            await app.UseConfigurationMiddleWare();

            app.MapControllers();




            app.Run();
        }
    }
}
