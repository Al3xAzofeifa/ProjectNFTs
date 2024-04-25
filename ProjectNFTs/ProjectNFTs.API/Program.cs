using Microsoft.EntityFrameworkCore;
using ProjectNFTs.Application.Profiles;
using ProjectNFTs.Application.Services.Implementations;
using ProjectNFTs.Application.Services.Interfaces;
using ProjectNFTs.Infraestructure.Data;
using ProjectNFTs.Infraestructure.Repository.Implementations;
using ProjectNFTs.Infraestructure.Repository.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//D.I
builder.Services.AddTransient<IServiceNft, ServiceNft>();
builder.Services.AddTransient<IRepositoryNft, RepositoryNft>();

builder.Services.AddTransient<IServiceCliente, ServiceCliente>();
builder.Services.AddTransient<IRepositoryCliente, RepositoryCliente>();

builder.Services.AddTransient<IServicePurchase, ServicePurchase>();
builder.Services.AddTransient<IRepositoryPurchase, RepositoryPurchase>();


//Automapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<NftProfile>();
    config.AddProfile<ClienteProfile>();
    config.AddProfile<PurchaseProfile>();
});

// Config Connection to SQLServer DataBase
builder.Services.AddDbContext<ProjectNFTsContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerDataBase"));

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

//// Configure Swagger 
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Configure Swagger 
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
