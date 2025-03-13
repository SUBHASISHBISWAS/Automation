#region

using Alstom.Spectrail.Server.Common.Contracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#endregion

namespace Alstom.Spectrail.Server.Common.Entities;

public abstract class EntityBase : IEntityBase
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    public string? CreatedBy { get; set; } = Environment.UserName ?? Environment.MachineName ?? "Unknown";
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string? LastModifiedBy { get; set; } = Environment.UserName ?? Environment.MachineName ?? "Unknown";
    public DateTime? LastModifiedDate { get; set; } = DateTime.Now;

    [BsonElement("checksum")] public string? Checksum { get; set; } // ✅ Checksum for change detection
}