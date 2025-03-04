#region

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#endregion

namespace SpectrailTestDataProvider.Domain.Common;

public abstract class EntityBase
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string? CreatedBy { get; set; } = "System";
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string? LastModifiedBy { get; set; } = "System";
    public DateTime? LastModifiedDate { get; set; } = DateTime.Now;
}