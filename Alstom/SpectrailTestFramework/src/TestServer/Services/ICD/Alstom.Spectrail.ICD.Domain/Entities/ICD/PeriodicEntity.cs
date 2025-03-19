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
// FileName: PeriodicEntity.cs
// ProjectName: Alstom.Spectrail.ICD.Domain
// Created by SUBHASISH BISWAS On: 2025-03-19
// Updated by SUBHASISH BISWAS On: 2025-03-19
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.Server.Common.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#endregion

namespace Alstom.Spectrail.ICD.Domain.Entities.ICD;

public class PeriodicEntity : EntityBase
{
    [BsonElement("VariableAddress")] public string? VariableAddress { get; set; }

    [BsonElement("SignalOffset")] public int? SignalOffset { get; set; }

    [BsonElement("SignalName")] public string? SignalName { get; set; }

    [BsonElement("Control_Build_Type")] public string? Control_Build_Type { get; set; }

    [BsonElement("ComplexNetworkType")] public string? ComplexNetworkType { get; set; }

    [BsonElement("SILLevel")] public string? SILLevel { get; set; }

    [BsonElement("SystemContent")] public string? SystemContent { get; set; }

    [BsonElement("SignalDescription")] public string? SignalDescription { get; set; }

    [BsonElement("SenderFunction")] public string? SenderFunction { get; set; }

    [BsonElement("SenderEquipment")] public string? SenderEquipment { get; set; }

    [BsonElement("ReceiverEquipment")] public string? ReceiverEquipment { get; set; }

    [BsonElement("ReceiverFunctionorLSequipmentorCANOPENCOBID")]
    public string? ReceiverFunctionorLSequipmentorCANOPENCOBID { get; set; }

    [BsonRepresentation(BsonType.String)] // Ensures it stores as a string
    public string? DefaultValue { get; set; }

    [BsonElement("MultiplierValue")] public double? MultiplierValue { get; set; }

    [BsonElement("ShiftValue")] public double? ShiftValue { get; set; }

    [BsonElement("TaskName")] public string? TaskName { get; set; }

    [BsonElement("Alignment")] public string? Alignment { get; set; }

    [BsonElement("Size")] public int? Size { get; set; }

    [BsonElement("RemoteSignalOffset")] public int? RemoteSignalOffset { get; set; }

    [BsonElement("PrimaryReadbackDataSignal")]
    public string? PrimaryReadbackDataSignal { get; set; }

    [BsonElement("ApplicableToConfiguration")]
    public string? ApplicableToConfiguration { get; set; }
}