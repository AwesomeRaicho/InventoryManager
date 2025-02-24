using InventoryManager.Core.Interfaces;
using InventoryManager.Core.Models.Identity;
using InventoryManager.Core.Services;
using InventoryManager.Infrastructure.DataAccess;
using InventoryManager.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);




// Add services to the container.

builder.Services.AddSingleton<IExceptionHandling, ExceptionHandling>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductInstanceService, ProductInstanceService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();



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


var app = builder.Build();

app.UseExceptionHandlerMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
