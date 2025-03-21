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
// Updated by SUBHASISH BISWAS On: 2025-03-21
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.API.Middleware;
using Alstom.Spectrail.ICD.Application;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Handlers;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Handler;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using Alstom.Spectrail.ICD.Application.Models;
using Alstom.Spectrail.ICD.Application.Registry;
using Alstom.Spectrail.ICD.Infrastructure;
using Alstom.Spectrail.ICD.Infrastructure.Persistence.Contexts.Mongo;
using Alstom.Spectrail.Server.Common.Configuration;
using Alstom.Spectrail.Server.Common.Entities;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
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

    var tempProvider = builder.Services.BuildServiceProvider();
    var registry = tempProvider.GetRequiredService<EntityRegistry>();

    var dynamicTypes = registry.RegisterEntity(); // This builds types & updates the DB

    container.RegisterType<RepositoryQueryHandler>()
        .As<IRequestHandler<RepositoryQuery, IEnumerable<EntityBase>>>()
        .InstancePerLifetimeScope();

    foreach (var entityType in dynamicTypes)
    {
        var commandType = typeof(RepositoryCommand<>).MakeGenericType(entityType);
        var commandHandlerType = typeof(RepositoryCommandHandler<>).MakeGenericType(entityType);
        var commandInterface = typeof(IRequestHandler<,>).MakeGenericType(commandType, typeof(bool));

        container.RegisterType(commandHandlerType)
            .As(commandInterface)
            .InstancePerLifetimeScope();

        Console.WriteLine($"✅ Registered {commandHandlerType.Name} into Autofac");
    }
});

// ✅ Register core services
services.AddAutoMapper(typeof(Program));
services.AddSingleton<IServerConfigHelper, ServerConfigHelper>();

services.Configure<SpectrailMongoDatabaseSettings>(
    builder.Configuration.GetSection("SpectrailMongoDatabaseSettings"));

services.AddSingleton<ICDMongoDataContext>();

services.AddSingleton<EntityRegistry>(sp =>
    new EntityRegistry(
        sp.GetRequiredService<ICDMongoDataContext>(),
        sp.GetRequiredService<IServerConfigHelper>(),
        services // Passed for dynamic registration
    ));

//services.AddScoped<EntityRegistryOrchestrator>();

services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var connString = configuration["RedisConfig:ConnectionString"] ?? "localhost:6379,abortConnect=false";
    return ConnectionMultiplexer.Connect(connString);
});

// ✅ Register application and infrastructure services
services.RegisterApplicationServices();
services.RegisterInfrastructureServices(configuration);

// ✅ Run Entity Registry & Seed Data before app starts
await using (var tempProvider = services.BuildServiceProvider())
{
    tempProvider.GetRequiredService<EntityRegistry>();
}

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