#region

using Microsoft.OpenApi.Models;
using SpectrailTestDataProvider.API.Utility;
using SpectrailTestDataProvider.Application;
using SpectrailTestDataProvider.Infrastructure;

#endregion

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// ✅ Add AutoMapper
services.AddAutoMapper(typeof(Program));

ServerConfigHelper configHelper = new();
services.AddSingleton(configHelper);

// ✅ Add Application & Infrastructure Services
services.RegisterInfrastructureServices(configuration);
services.RegisterApplicationServices();

// ✅ Add Controllers & Swagger
services.AddControllers();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SpectrailTestDataProvider.API", Version = "v1" });
});


// ✅ Build App
var app = builder.Build();


// ✅ Configure Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpectrailTestDataProvider.API v1"));
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// ✅ Run Application
app.Run();