using Microsoft.EntityFrameworkCore;
using ProjectNFTs.Application.Profiles;
using ProjectNFTs.Application.Services.Implementations;
using ProjectNFTs.Application.Services.Interfaces;
using ProjectNFTs.Infraestructure.Data;
using ProjectNFTs.Infraestructure.Repository.Implementations;
using ProjectNFTs.Infraestructure.Repository.Interfaces;
using Serilog.Events;
using Serilog;
using ProjectNFTs.Web.Middleware;
using System.Text;
using ProjectNFTs.Application.Config;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args); 

// Mapping AppConfig Class to read  appsettings.json
builder.Services.Configure<AppConfig>(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();

//D.I
builder.Services.AddTransient<IRepositoryCliente, RepositoryCliente>();
builder.Services.AddTransient<IServiceCliente, ServiceCliente>();

builder.Services.AddTransient<IRepositoryPais, RepositoryPais>();
builder.Services.AddTransient<IServicePais, ServicePais>();

builder.Services.AddTransient<IServiceTarjeta, ServiceTarjeta>();
builder.Services.AddTransient<IRepositoryTarjeta, RepositoryTarjeta>();

builder.Services.AddTransient<IServiceNft, ServiceNft>();
builder.Services.AddTransient<IRepositoryNft, RepositoryNft>();

builder.Services.AddTransient<IServiceFactura, ServiceFactura>();
builder.Services.AddTransient<IRepositoryFactura, RepositoryFactura>();

builder.Services.AddTransient<IServiceUsuario, ServiceUsuario>();
builder.Services.AddTransient<IRepositoryUsuario, RepositoryUsuario>();

builder.Services.AddTransient<IServiceRol, ServiceRol>();
builder.Services.AddTransient<IRepositoryRol, RepositoryRol>();

builder.Services.AddTransient<IServiceReporte, ServiceReporte>();

builder.Services.AddTransient<IServicePurchase, ServicePurchase>();
builder.Services.AddTransient<IRepositoryPurchase, RepositoryPurchase>();


// Security
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(
            new ResponseCacheAttribute
            {
                NoStore = true,
                Location = ResponseCacheLocation.None,
            }
        );
});


//Automapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<ClienteProfile>();
    config.AddProfile<PaisProfile>();
    config.AddProfile<TarjetaProfile>();
    config.AddProfile<NftProfile>();
    config.AddProfile<FacturaProfile>();
    config.AddProfile<UsuarioProfile>();
    config.AddProfile<RolUsuarioProfile>();
    config.AddProfile<PurchaseProfile>();
});



// Config Connection to SQLServer DataBase
builder.Services.AddDbContext<ProjectNFTsContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerDataBase"));

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});


// Logger
var logger = new LoggerConfiguration()
                    //.MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                    .Enrich.FromLogContext()
                    //.WriteTo.Console(LogEventLevel.Verbose)
                    .WriteTo.Console(LogEventLevel.Information) // Para ver informacion más detallada
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File(@"Logs\Info-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.File(@"Logs\Debug-.log", shared: true, encoding: System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.File(@"Logs\Warning-.log", shared: true, encoding: System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.File(@"Logs\Error-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo.File(@"Logs\Fatal-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .CreateLogger();


builder.Host.UseSerilog(logger);

var app = builder.Build();
bool isDeployment = app.Environment.IsDevelopment();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Error control Middleware
    app.UseMiddleware<ErrorHandlingMiddleware>();
}

// Error access control 
app.UseStatusCodePagesWithReExecute("/error/{0}");

//Aplicacion de Serilog
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Activate Antiforgery DEBE COLOCARSE ACA!
app.UseAntiforgery();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
