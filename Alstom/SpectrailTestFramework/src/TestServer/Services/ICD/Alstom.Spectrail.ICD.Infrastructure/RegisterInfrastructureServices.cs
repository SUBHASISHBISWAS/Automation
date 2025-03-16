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
// FileName: RegisterInfrastructureServices.cs
// ProjectName: Alstom.Spectrail.ICD.Infrastructure
// Created by SUBHASISH BISWAS On: 2025-03-11
// Updated by SUBHASISH BISWAS On: 2025-03-16
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Models;
using Alstom.Spectrail.ICD.Infrastructure.Persistence.Contexts.Mongo;
using Alstom.Spectrail.ICD.Infrastructure.Persistence.Drivers;
using Alstom.Spectrail.ICD.Infrastructure.Repository;
using Alstom.Spectrail.Server.Common.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Alstom.Spectrail.ICD.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SpectrailMongoDatabaseSettings>(options =>
            configuration.GetSection("SpectrailMongoDatabaseSettings").Bind(options));
        services.AddScoped<IICDDbContext, ICDMongoDataContext>();
        services.AddScoped(typeof(IDataProvider<>), typeof(MongoDataProvider<>));
        services.AddScoped(typeof(IAsyncRepository<>), typeof(ICDRepository<>));
        services.AddScoped(typeof(IICDRepository<>), typeof(ICDRepository<>));

        return services;
    }
}