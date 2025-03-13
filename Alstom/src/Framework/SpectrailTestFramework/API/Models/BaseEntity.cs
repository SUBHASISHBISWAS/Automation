#region

using Newtonsoft.Json;

#endregion

namespace SpectrailTestFramework.API.Models;

/// <summary>
///     ✅ Base class for all entities that include auditing fields.
/// </summary>
public abstract class BaseEntity
{
    [JsonProperty("createdBy")] public string CreatedBy { get; set; }

    [JsonProperty("createdDate")] public DateTime CreatedDate { get; set; }

    [JsonProperty("lastModifiedBy")] public string LastModifiedBy { get; set; }

    [JsonProperty("lastModifiedDate")] public DateTime LastModifiedDate { get; set; }
}