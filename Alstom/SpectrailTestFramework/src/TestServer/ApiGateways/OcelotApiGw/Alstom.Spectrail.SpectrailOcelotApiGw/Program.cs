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
// ProjectName: Alstom.Spectrail.SpectrailOcelotApiGw
// Created by SUBHASISH BISWAS On: 2025-03-18
// Updated by SUBHASISH BISWAS On: 2025-03-18
//  ******************************************************************************/

#endregion

#region

using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

#endregion

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configure app settings (Replaces ConfigureAppConfiguration)
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);

// 🔹 Configure Logging (Replaces ConfigureLogging)
builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 🔹 Add Ocelot and CacheManager
builder.Services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

await app.UseOcelot();

app.Run();