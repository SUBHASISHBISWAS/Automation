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
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.API.Middleware;
using Alstom.Spectrail.ICD.Application;
using Alstom.Spectrail.ICD.Infrastructure;
using Alstom.Spectrail.Server.Common.Configuration;
using Microsoft.OpenApi.Models;

#endregion

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// ✅ Add AutoMapper
services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton<IServerConfigHelper, ServerConfigHelper>();

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
app.MapGet("/health", () => Results.Ok("Healthy"));
// ✅ Run Application
app.Run();