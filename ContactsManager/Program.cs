using Services;
using ServicesContracts;
using Microsoft.EntityFrameworkCore;
using Entities;
using RepositoryContracts;
using Repositories;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;

namespace Contacts_Manager
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Serilog
            builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
                loggerConfiguration.ReadFrom.Services(services);
            });
            builder.Services.AddControllersWithViews();

            // Adding services to the IoC Container
            builder.Services.AddScoped<IPersonService, PersonsService>();
            builder.Services.AddTransient<ICountriesRepository, CountriesRepository>();
            builder.Services.AddTransient<IPersonsRepository, PersonsRepository>();
            builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddHttpLogging(options =>
            {
                options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
            });
            var app = builder.Build();

            app.UseSerilogRequestLogging();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            if(builder.Environment.IsEnvironment("Test") == false)
                Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
            
            //Logging messages explicitly
            //app.Logger.LogDebug("debug-message");
            //app.Logger.LogInformation("information-message");
            //app.Logger.LogWarning("warning-message");
            //app.Logger.LogError("error-message");
            //app.Logger.LogCritical("critical-message");

            app.UseHttpLogging();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

            app.Run();
        }
    }
}