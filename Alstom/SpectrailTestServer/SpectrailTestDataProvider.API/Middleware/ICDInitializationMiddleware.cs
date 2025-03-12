#region

using System.Diagnostics;
using MediatR;
using SpectrailTestDataProvider.Application.Features.ICD.Commands.Command;
using SpectrailTestDataProvider.Shared.Configuration;

#endregion

namespace SpectrailTestDataProvider.API.Middleware;

/// <summary>
///     ✅ Middleware to Seed ICD Data on Application Startup based on Feature Flags
/// </summary>
public class ICDSeedDataMiddleware(
    RequestDelegate next,
    IServiceScopeFactory scopeFactory,
    ServerConfigHelper configHelper)
{
    private static bool _initialized;

    public async Task Invoke(HttpContext context)
    {
        if (!_initialized && configHelper.IsFeatureEnabled("EnableMiddlewarePreloading"))
        {
            Debug.WriteLine("🚀 Middleware Preloading Enabled: Seeding ICD Data...");

            using var scope = scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var success = await mediator.Send(new SeedICDDataCommand());

            if (success)
            {
                _initialized = true;
                Debug.WriteLine("✅ ICD Data Seeding Completed via Middleware!");
            }
            else
            {
                Debug.WriteLine("⚠️ ICD Data Seeding Failed via Middleware!");
            }
        }

        await next(context);
    }
}