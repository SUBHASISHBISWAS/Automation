#region

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SpectrailTestDataProvider.Domain.Contract;

#endregion

namespace SpectrailTestDataProvider.Domain.Common;

public abstract class EntityBase : IEntityBase
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    public string? CreatedBy { get; set; } = "System";
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string? LastModifiedBy { get; set; } = "System";
    public DateTime? LastModifiedDate { get; set; } = DateTime.Now;

    [BsonElement("checksum")] public string? Checksum { get; set; } // ✅ Checksum for change detection
}