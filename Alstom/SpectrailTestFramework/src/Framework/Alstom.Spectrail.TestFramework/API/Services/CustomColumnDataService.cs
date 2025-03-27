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
// FileName: CustomColumnDataService.cs
// ProjectName: Alstom.Spectrail.TestFramework
// Created by SUBHASISH BISWAS On: 2025-03-19
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.TestFramework.API.APIClient;
using Alstom.Spectrail.TestFramework.API.Models;
using Newtonsoft.Json;
using RestSharp;

#endregion

namespace Alstom.Spectrail.TestFramework.API.Services;

public class CustomColumnDataService(ApiClient client) : IApiService
{
    public async Task<List<CustomColumnRecords>?> GetCustomColumnByEquipment(string fileName, string sheetName)
    {
        var url =
            $"https://localhost:5000/api/v1/CustomColumn/GetCustomColumnByEquipment?fileName={fileName}&sheetName={sheetName}";
        var otherUrl = $"/GetCustomColumnByEquipment?fileName={fileName}&sheetName={sheetName}";
        var request = new RestRequest(url);
        var response = await client.ExecuteAsync(request);

        if (!response.IsSuccessful) throw new Exception($"❌ API Error: {response.ErrorMessage}");

        // ✅ Deserialize API response to List<CustomColumnRecords>
        return JsonConvert.DeserializeObject<List<CustomColumnRecords>>(response.Content ??
                                                                        throw new InvalidOperationException());
    }
}