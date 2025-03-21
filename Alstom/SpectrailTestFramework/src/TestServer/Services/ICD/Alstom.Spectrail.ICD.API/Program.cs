#region ©COPYRIGHT

// /*******************************************************************************
//  *   © COPYRIGHT ALSTOM SA 2025 - ALL RIGHTS RESERVED
//  *
//  *  This file is part of the Alstom Spectrail project.
//  *  Unauthorized copying, modification, distribution, or use of this file,
//  *  via any medium, is strictly prohibited.
//  *
//  *  Proprietary and confidential.
//  *  Created and maintained by Alstom for internal use in the Spectrail project.
//  ******************************************************************************/
// 
//  /*******************************************************************************
// AuthorName: SUBHASISH BISWAS
// Email: subhasish.biswas@alstomgroup.com
// FileName: Program.cs
// ProjectName: Alstom.Spectrail.ICD.API
// Created by SUBHASISH BISWAS On: 2025-03-11
// Updated by SUBHASISH BISWAS On: 2025-03-22
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.API.Middleware;
using Alstom.Spectrail.ICD.Application;
using Alstom.Spectrail.ICD.Application.Models;
using Alstom.Spectrail.ICD.Application.Registry;
using Alstom.Spectrail.ICD.Infrastructure;
using Alstom.Spectrail.ICD.Infrastructure.Persistence.Contexts.Mongo;
using Alstom.Spectrail.Server.Common.Configuration;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

#endregion

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// 🔁 Use Autofac instead of built-in DI
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    // Autofac registration for orchestrator with ILifetimeScope support
    container.RegisterType<EntityRegistryOrchestrator>()
        .AsSelf()
        .InstancePerLifetimeScope();
});


services.AddSingleton<IServerConfigHelper, ServerConfigHelper>();

services.Configure<SpectrailMongoDatabaseSettings>(
    builder.Configuration.GetSection("SpectrailMongoDatabaseSettings"));

services.AddSingleton<ICDMongoDataContext>();

var mapperConfigExpression = new MapperConfigurationExpression();
services.AddSingleton<IMapper>(sp =>
{
    var config = new MapperConfiguration(mapperConfigExpression);
    return config.CreateMapper(sp.GetRequiredService);
});
services.AddSingleton<EntityRegistry>(sp =>
        new EntityRegistry(
            sp.GetRequiredService<ICDMongoDataContext>(),
            sp.GetRequiredService<IServerConfigHelper>(),
            services, mapperConfigExpression) // Passed for dynamic registration
);


services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var connString = configuration["RedisConfig:ConnectionString"] ?? "localhost:6379,abortConnect=false";
    return ConnectionMultiplexer.Connect(connString);
});

// ✅ Register application and infrastructure services
services.RegisterApplicationServices();
services.RegisterInfrastructureServices(configuration);

// ✅ Register controllers and Swagger
services.AddControllers();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Alstom.Spectrail.ICD.API",
        Version = "v1"
    });
});

// ✅ Build the app
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<EntityRegistry>();
    var orchestrator = scope.ServiceProvider.GetRequiredService<EntityRegistryOrchestrator>();
    await orchestrator.ExecuteAsync(true);
}

// ✅ Run one-time entity registration and seed check via middleware
app.UseMiddleware<EntityRegistrationMiddleware>();

// ✅ Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Alstom.Spectrail.ICD.API v1"));
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();