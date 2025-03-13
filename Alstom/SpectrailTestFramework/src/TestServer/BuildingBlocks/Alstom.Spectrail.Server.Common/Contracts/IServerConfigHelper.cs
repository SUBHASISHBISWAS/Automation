namespace Alstom.Spectrail.Server.Common.Configuration;

/// <summary>
/// ✅ Interface for `ServerConfigHelper`, useful for dependency injection and testing.
/// </summary>
public interface IServerConfigHelper
{
    /// <summary>
    /// ✅ Retrieves the ICD folder path and ensures it's accessible.
    /// </summary>
    string GetICDFolderPath();

    /// <summary>
    /// ✅ Retrieves a list of all ICD Excel files in the ICD folder.
    /// </summary>
    List<string> GetICDFiles();

    /// <summary>
    /// ✅ Checks if a feature flag is enabled (Defaults to false if missing).
    /// </summary>
    bool IsFeatureEnabled(string feature);
}