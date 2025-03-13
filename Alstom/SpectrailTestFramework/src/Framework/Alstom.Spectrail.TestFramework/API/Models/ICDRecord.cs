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
// FileName: ICDRecord.cs
// ProjectName: Alstom.Spectrail.TestFramework
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.TestFramework.API.Models;
using Newtonsoft.Json;

#endregion

namespace SpectrailTestDataProvider.Domain.Entities.ICD;

public class ICDRecord : BaseEntity
{
    [JsonProperty("variableAddress")] public string? VariableAddress { get; set; }
    [JsonProperty("signalOffset")] public string? SignalOffset { get; set; }
    [JsonProperty("signalName")] public string? SignalName { get; set; }
    [JsonProperty("controlBuildType")] public string? ControlBuildType { get; set; }
    [JsonProperty("complexNetworkType")] public string? ComplexNetworkType { get; set; }
    [JsonProperty("silLevel")] public string? SILLevel { get; set; }
    [JsonProperty("systemContent")] public string? SystemContent { get; set; }
    [JsonProperty("signalDescription")] public string? SignalDescription { get; set; }
    [JsonProperty("senderFunction")] public string? SenderFunction { get; set; }
    [JsonProperty("senderEquipment")] public string? SenderEquipment { get; set; }
    [JsonProperty("receiverEquipment")] public string? ReceiverEquipment { get; set; }
    [JsonProperty("receiverFunction")] public string? ReceiverFunction { get; set; }
    [JsonProperty("defaultValue")] public string? DefaultValue { get; set; }
    [JsonProperty("multiplierValue")] public string? MultiplierValue { get; set; }
    [JsonProperty("shiftValue")] public string? ShiftValue { get; set; }
    [JsonProperty("taskName")] public string? TaskName { get; set; }
    [JsonProperty("alignment")] public string? Alignment { get; set; }
    [JsonProperty("size")] public string? Size { get; set; }
    [JsonProperty("remoteSignalOffset")] public string? RemoteSignalOffset { get; set; }

    [JsonProperty("primaryReadbackDataSignal")]
    public string? PrimaryReadbackDataSignal { get; set; }

    [JsonProperty("applicableToConfiguration")]
    public string? ApplicableToConfiguration { get; set; }
}