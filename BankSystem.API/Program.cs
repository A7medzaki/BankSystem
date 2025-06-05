using BankSystem.API.Extension;
using BankSystem.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Configuring DbContext to use SQL Server with connection string
            builder.Services.AddDbContext<BankingContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Add custom services and repositories
            builder.Services.AddApplicationServices();

            // Add controllers (API endpoint handling)
            builder.Services.AddControllers();

            // Swagger configuration for API documentation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Build the application
            var app = builder.Build();

            // Configure the HTTP request pipeline (Swagger UI and others)
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();  // Enable Swagger endpoint
                app.UseSwaggerUI();  // Enable Swagger UI
            }

            app.UseStaticFiles();  // Enable static files to be served
            app.UseHttpsRedirection();  // Redirect HTTP to HTTPS

            app.UseAuthorization();  // Authorization middleware

            app.MapControllers();  // Map API controllers

            // Run the application
            app.Run();
        }
    }
}
