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
// FileName: ICDDataService.cs
// ProjectName: Alstom.Spectrail.TestFramework
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-17
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.TestFramework.API.APIClient;
using Newtonsoft.Json;
using RestSharp;
using SpectrailTestDataProvider.Domain.Entities.ICD;

#endregion

namespace Alstom.Spectrail.TestFramework.API.Services;

public class ICDDataService(ApiClient client) : IApiService
{
    public async Task<List<ICDRecord>?> GetICDDataAsync()
    {
        var request = new RestRequest("/all?fileName=trdp_icd_generated");
        var response = await client.ExecuteAsync(request);

        //request = new RestRequest("/all");
        //response = await client.ExecuteAsync(request);

        if (!response.IsSuccessful) throw new Exception($"❌ API Error: {response.ErrorMessage}");

        // ✅ Deserialize API response to List<ICDRecord>
        return JsonConvert.DeserializeObject<List<ICDRecord>>(response.Content ??
                                                              throw new InvalidOperationException());
    }
}