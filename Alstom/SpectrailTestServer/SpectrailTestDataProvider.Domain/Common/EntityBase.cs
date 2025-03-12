#region

using MongoDB.Bson.Serialization.Attributes;
using SpectrailTestDataProvider.Domain.Contract;

#endregion

namespace SpectrailTestDataProvider.Domain.Common;

public abstract class EntityBase : IEntityBase
{
    /*[BsonId] // ✅ Marks it as the primary key
    [BsonRepresentation(BsonType.ObjectId)] // ✅ Ensures MongoDB stores it correctly
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString(); */ // ✅ Generate new ObjectId when needed

    public string? CreatedBy { get; set; } = "System";
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string? LastModifiedBy { get; set; } = "System";
    public DateTime? LastModifiedDate { get; set; } = DateTime.Now;

    [BsonElement("checksum")] public string? Checksum { get; set; } // ✅ Checksum for change detection
}