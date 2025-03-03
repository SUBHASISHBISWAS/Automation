#region

using Microsoft.OpenApi.Models;
using SpectrailTestDataProvider.Application;
using SpectrailTestDataProvider.Infrastructure;

#endregion

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// ✅ Add AutoMapper
services.AddAutoMapper(typeof(Program));


// ✅ Add Application & Infrastructure Services
services.AddApplicationServices();
services.AddInfrastructureServices(configuration);

// ✅ Add Controllers & Swagger
services.AddControllers();
services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering.API", Version = "v1" }); });

foreach (var descriptor in builder.Services)
    Console.WriteLine($"{descriptor.ServiceType} -> {descriptor.ImplementationType}");

// ✅ Build App
var app = builder.Build();


// ✅ Configure Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.API v1"));
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// ✅ Run Application
app.Run();