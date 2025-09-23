using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using ProjectLaborBackend;
using ProjectLaborBackend.Profiles;
using ProjectLaborBackend.Services;
using ProjectLaborBackend.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
namespace ProjectLaborBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DbContext
            builder.Services.AddDbContext<AppDbContext>();


            // Add services to the container.
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IWarehouseService, WarehouseService>();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            
            //Automapper maps
            builder.Services.AddAutoMapper(cfg => { }, typeof(ProductProfile));
            builder.Services.AddAutoMapper(cfg => { }, typeof(WarehouseProfile));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAutoMapper(cfg => { }, typeof(UserProfile));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Labor API v1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
