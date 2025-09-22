using ProjectLaborBackend;
using ProjectLaborBackend.Profiles;
using ProjectLaborBackend.Services;
using ProjectLaborBackend.Entities;
namespace ProjectLaborBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>();
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddAutoMapper(cfg => { }, typeof(WarehouseProfile));
            builder.Services.AddScoped<IWarehouseService, WarehouseService>();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
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
