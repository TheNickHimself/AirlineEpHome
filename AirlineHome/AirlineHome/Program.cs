using Data.DataContext;
using Data.Repository;
//using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AirlineHome
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            /*
            builder.Services.AddDbContext<AirlineDbContext>(options =>
            options.UseSqlServer("Server=MOTHERSHIP\\SQLEXPRESS01;Database=airlinedb;Trusted_Connection=True;MultipleActiveResultSets=tru", o => o.TrustServerCertificate()));
            */
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<AirlineDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //builder.Services.AddDbContext<AirlineDbContext>(options =>options.UseSqlServer(AirlineDbContext));

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            //injextor
            builder.Services.AddScoped<FlightDbRepository>();
            builder.Services.AddScoped<TicketDBRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();

        }
    }

}


