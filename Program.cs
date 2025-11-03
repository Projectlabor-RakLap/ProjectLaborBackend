using ProjectLaborBackend.Email;
using ProjectLaborBackend.Entities;
using ProjectLaborBackend.Profiles;
using ProjectLaborBackend.Services;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ProjectLaborBackend.Controllers;

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
            builder.Services.AddScoped<IStockService, StockService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IStockChangeService, StockChangeService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddFluentEmail(builder.Configuration);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            //Automapper maps
            builder.Services.AddAutoMapper(cfg => { }, typeof(ProductProfile));
            builder.Services.AddAutoMapper(cfg => { }, typeof(WarehouseProfile));
            builder.Services.AddAutoMapper(cfg => { }, typeof(StockProfile));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddAutoMapper(cfg => { }, typeof(UserProfile));

            builder.WebHost.ConfigureKestrel(options =>
            {
                // HTTP minden IP-re (pl. 10.100.0.66, localhost)
                options.ListenAnyIP(5116);

                // HTTPS minden IP-re
                options.ListenAnyIP(7116, listenOptions =>
                {
                    listenOptions.UseHttps();
                });
            });


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

            app.UseCors(builder =>
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader());

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
