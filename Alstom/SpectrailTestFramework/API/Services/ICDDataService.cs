#region

using RestSharp;
using SpectrailTestFramework.API.APIClient;
using SpectrailTestFramework.Services;

#endregion

namespace SpectrailTestFramework.API.Services;

public class ICDDataService(ApiClient client) : IApiService
{
    public async Task<RestResponse> GetDataAsync(string endpoint)
    {
        var request = new RestRequest(endpoint);
        return await client.ExecuteAsync(request);
    }
}