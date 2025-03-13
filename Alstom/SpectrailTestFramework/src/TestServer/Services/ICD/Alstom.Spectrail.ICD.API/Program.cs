#region

using Microsoft.OpenApi.Models;
using Alstom.Spectrail.ICD.API.Middleware;
using Alstom.Spectrail.ICD.Application;
using SpectrailTestDataProvider.Infrastructure;
using Alstom.Spectrail.Server.Common.Configuration;

#endregion

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// ✅ Add AutoMapper
services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton<ServerConfigHelper>();

// ✅ Add Application & Infrastructure Services
services.RegisterApplicationServices();
services.RegisterInfrastructureServices(configuration);


// ✅ Add Controllers & Swagger
services.AddControllers();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Alstom.Spectrail.ICD.API", Version = "v1" });
});


// ✅ Build App
var app = builder.Build();

app.UseMiddleware<ICDSeedDataMiddleware>();

// ✅ Configure Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Alstom.Spectrail.ICD.API v1"));
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// ✅ Run Application
app.Run();