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
// FileName: EntityBase.cs
// ProjectName: Alstom.Spectrail.Server.Common
// Created by SUBHASISH BISWAS On: 2025-03-11
// Updated by SUBHASISH BISWAS On: 2025-03-26
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.Server.Common.Contracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#endregion

namespace Alstom.Spectrail.Server.Common.Entities;

public class EntityBase : IEntityBase
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    public string? CreatedBy { get; set; } = Environment.UserName ?? Environment.MachineName ?? "Unknown";
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string? LastModifiedBy { get; set; } = Environment.UserName ?? Environment.MachineName ?? "Unknown";
    public DateTime? LastModifiedDate { get; set; } = DateTime.Now;
    public string? FileKey { get; set; }
    [BsonElement("FileChecksum")] public string? FileChecksum { get; set; }

    // ✅ Added properties
    public string FileName { get; set; } = string.Empty;

    public string SheetName { get; set; } = string.Empty;

    [BsonElement("RowChecksum")] public string? RowChecksum { get; set; }
    // ✅ Checksum for change detection
}