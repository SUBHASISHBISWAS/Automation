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
        var request = new RestRequest("/import-excel?filePath=C:\\Data\\DCU_Data.xlsx&sheetName=Sheet1");
        var response = await client.ExecuteAsync(request);

        request = new RestRequest("/all");
        response = await client.ExecuteAsync(request);

        if (!response.IsSuccessful) throw new Exception($"❌ API Error: {response.ErrorMessage}");

        // ✅ Deserialize API response to List<ICDRecord>
        return JsonConvert.DeserializeObject<List<ICDRecord>>(response.Content ??
                                                              throw new InvalidOperationException());
    }
}