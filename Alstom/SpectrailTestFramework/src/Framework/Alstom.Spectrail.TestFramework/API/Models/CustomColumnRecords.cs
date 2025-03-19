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
// FileName: CustomColumnRecords.cs
// ProjectName: Alstom.Spectrail.TestFramework
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-19
//  ******************************************************************************/

#endregion

#region

using Newtonsoft.Json;

#endregion

namespace Alstom.Spectrail.TestFramework.API.Models;

public class CustomColumnRecords
{
    [JsonProperty(nameof(RootProducerFunction))]
    public string? RootProducerFunction { get; set; }

    [JsonProperty(nameof(MaxValue))] public string? MaxValue { get; set; }

    [JsonProperty(nameof(DataFlows))] public string? DataFlows { get; set; }

    [JsonProperty(nameof(ProducerFunction))]
    public string? ProducerFunction { get; set; }

    [JsonProperty(nameof(ConsumerFunction))]
    public string? ConsumerFunction { get; set; }

    [JsonProperty(nameof(EventName))] public string? EventName { get; set; }

    [JsonProperty(nameof(RootConsumerFunction))]
    public string? RootConsumerFunction { get; set; }

    [JsonProperty(nameof(MinValue))] public string? MinValue { get; set; }
}