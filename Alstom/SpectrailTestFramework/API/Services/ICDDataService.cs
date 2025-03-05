#region

using Newtonsoft.Json;
using RestSharp;
using SpectrailTestFramework.API.APIClient;
using SpectrailTestFramework.API.Models;
using SpectrailTestFramework.Services;

#endregion

namespace SpectrailTestFramework.API.Services;

public class ICDDataService(ApiClient client) : IApiService
{
    public async Task<List<ICDRecord>?> GetICDDataAsync()
    {
        var request = new RestRequest("/ICD");
        var response = await client.ExecuteAsync(request);

        if (!response.IsSuccessful) throw new Exception($"❌ API Error: {response.ErrorMessage}");

        // ✅ Deserialize API response to List<ICDRecord>
        return JsonConvert.DeserializeObject<List<ICDRecord>>(response.Content ??
                                                              throw new InvalidOperationException());
    }
}