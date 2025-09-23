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

            // Add DbContext
            builder.Services.AddDbContext<AppDbContext>();

            // Add services to the container.
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IWarehouseService, WarehouseService>();
            builder.Services.AddScoped<IStockService, StockService>();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            
            //Automapper maps
            builder.Services.AddAutoMapper(cfg => { }, typeof(ProductProfile));
            builder.Services.AddAutoMapper(cfg => { }, typeof(WarehouseProfile));
            builder.Services.AddAutoMapper(cfg => { }, typeof(StockProfile));

            builder.Services.AddEndpointsApiExplorer();
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
