#region

using Newtonsoft.Json;

#endregion

namespace SpectrailTestFramework.API.Models;

/// <summary>
///     ✅ ICD Record class inheriting common properties from BaseEntity.
/// </summary>
public class ICDRecord : BaseEntity
{
    [JsonProperty("icdName")] public string IcdName { get; set; }

    [JsonProperty("icdDescription")] public string IcdDescription { get; set; }
}