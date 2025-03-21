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
// FileName: CustomColumnDto.cs
// ProjectName: Alstom.Spectrail.ICD.Domain
// Created by SUBHASISH BISWAS On: 2025-03-04
// Updated by SUBHASISH BISWAS On: 2025-03-22
//  ******************************************************************************/

#endregion

#region

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#endregion

namespace Alstom.Spectrail.ICD.Domain.DTO.ICD;

public class CustomColumnDto
{
    [BsonElement("RootProducerFunction")] public string? RootProducerFunction { get; set; }

    [BsonElement("MaxValue")] public string? MaxValue { get; set; }

    [BsonElement("DataFlows")] public string? DataFlows { get; set; }

    [BsonElement("ProducerFunction")] public string? ProducerFunction { get; set; }

    [BsonElement("ConsumerFunction")] public string? ConsumerFunction { get; set; }

    [BsonElement("EventName")] public string? EventName { get; set; }

    [BsonElement("RootConsumerFunction")] public string? RootConsumerFunction { get; set; }

    [BsonRepresentation(BsonType.String)] public string? MinValue { get; set; }
}