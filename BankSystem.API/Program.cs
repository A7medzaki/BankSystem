
using BankSystem.API.Extension;
using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Repository.Repositories;
using BankSystem.Repository.RepositoryInterfaces;
using BankSystem.Service.Helper;
using BankSystem.Service.Services.AccountService;
using BankSystem.Service.Services.Security;
using BankSystem.Service.Services.SubscriptionService;
using BankSystem.Service.Services.TransactionService;
using BankSystem.Service.Services.UserService;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<BankingContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });


            builder.Services.AddApplicationServices();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
