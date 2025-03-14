using InventoryManager.Core.Enums;
using InventoryManager.Core.Interfaces;
using InventoryManager.Core.Models.Identity;
using InventoryManager.Core.Services;
using InventoryManager.Infrastructure.DataAccess;
using InventoryManager.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);




// Add services to the container.

builder.Services.AddSingleton<IExceptionHandling, ExceptionHandling>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductInstanceService, ProductInstanceService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IPropertyTypeService, PropertyTypeService>();
builder.Services.AddScoped<IProduct_PropertyService, Product_PropertyService>();



builder.Services.AddDbContext<EntityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultDevConnection"]);
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<EntityDbContext>()
    .AddDefaultTokenProviders(); 

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddControllers(options =>
{
    //options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

var app = builder.Build();

app.UseExceptionHandlerMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend"); 
app.UseAuthorization();

app.MapControllers();
app.Run();
