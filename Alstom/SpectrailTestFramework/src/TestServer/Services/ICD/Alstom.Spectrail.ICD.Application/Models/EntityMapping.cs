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
// FileName: EntityMapping.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-17
// Updated by SUBHASISH BISWAS On: 2025-03-26
//  ******************************************************************************/

#endregion

#region

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#endregion

namespace Alstom.Spectrail.ICD.Application.Models;

/// <summary>
///     ✅ Represents an entity mapping stored in MongoDB.
/// </summary>
public class EntityMapping
{
    [BsonId] // ✅ Marks _id as MongoDB's primary key (auto-generated)
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString(); // ✅ No fixed _id!

    public string FileName { get; init; } = string.Empty;
    public string SheetName { get; init; } = string.Empty;
    public string EntityName { get; init; } = string.Empty;

    public bool? IsRegistered { get; init; } = false;
}