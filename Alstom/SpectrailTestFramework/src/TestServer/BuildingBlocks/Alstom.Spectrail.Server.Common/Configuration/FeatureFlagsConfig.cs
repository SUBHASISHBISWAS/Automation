namespace Alstom.Spectrail.Server.Common.Configuration;

/// <summary>
/// ✅ Represents feature flags from `appsettings.json`.
/// </summary>
public class FeatureFlagsConfig
{
    public bool EnableChecksumValidation { get; init; } = false;
    public bool EnableEagerLoading { get; init; } = false;
    public bool EnableMiddlewarePreloading { get; init; } = false;
}