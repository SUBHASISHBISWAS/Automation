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
// FileName: ApiServiceFactory.cs
// ProjectName: Alstom.Spectrail.TestFramework
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.TestFramework.API.APIClient;
using Alstom.Spectrail.TestFramework.API.Services;
using Alstom.Spectrail.TestFramework.Utilities;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Alstom.Spectrail.TestFramework.Factory;

public class ApiServiceFactory
{
    private readonly Dictionary<string, string> _apiBaseUrls;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _services;

    public ApiServiceFactory(IServiceProvider serviceProvider, ConfigHelper configHelper)
    {
        _serviceProvider = serviceProvider;

        // ✅ Read entire "ApiClients" section from appsettings.json dynamically
        var apiClientsConfig = configHelper.GetSection("ServerSettings:ApiClients").GetChildren();

        // ✅ Convert the config section to a dictionary dynamically
        _apiBaseUrls = apiClientsConfig.ToDictionary(k => k.Key, v => v.Value!);

        if (_apiBaseUrls.Count == 0) throw new InvalidOperationException("❌ No API clients found in configuration.");

        // ✅ Automatically register all services implementing `IApiService`
        _services = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IApiService).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToDictionary(t => t.Name, t => t);
    }

    /// <summary>
    ///     ✅ Gets an API service dynamically based on name
    /// </summary>
    public IApiService GetService(string serviceName)
    {
        if (!_services.TryGetValue(serviceName, out var serviceType))
            throw new InvalidOperationException($"❌ API Service '{serviceName}' not found.");

        if (!_apiBaseUrls.TryGetValue(serviceName, out var baseUrl))
            throw new InvalidOperationException($"❌ API Base URL for {serviceName} not found in configuration.");

        // ✅ Create an instance of ApiClient dynamically
        var apiClient = new ApiClient(baseUrl);

        // ✅ Inject the correct API Client into the service
        return (IApiService)ActivatorUtilities.CreateInstance(_serviceProvider, serviceType, apiClient);
    }
}