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
// FileName: DCUEntity.cs
// ProjectName: Alstom.Spectrail.ICD.Domain
// Created by SUBHASISH BISWAS On: 2025-03-04
// Updated by SUBHASISH BISWAS On: 2025-03-17
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.Server.Common.Entities;

#endregion

namespace Alstom.Spectrail.ICD.Domain.Entities.ICD;

public class DCUEntity : EntityBase
{
    public string? VariableAddress { get; set; }
    public int? SignalOffset { get; set; }
    public string? SignalName { get; set; }
    public string? ControlBuildType { get; set; }
    public string? ComplexNetworkType { get; set; }
    public string? SILLevel { get; set; }
    public string? SystemContent { get; set; }
    public string? SignalDescription { get; set; }
    public string? SenderFunction { get; set; }
    public string? SenderEquipment { get; set; }
    public string? ReceiverEquipment { get; set; }
    public string? ReceiverFunction { get; set; }
    public string? DefaultValue { get; set; }
    public string? MultiplierValue { get; set; }
    public string? ShiftValue { get; set; }
    public string? TaskName { get; set; }
    public string? Alignment { get; set; }
    public string? Size { get; set; }
    public string? RemoteSignalOffset { get; set; }
    public string? PrimaryReadbackDataSignal { get; set; }
    public string? ApplicableToConfiguration { get; set; }
}