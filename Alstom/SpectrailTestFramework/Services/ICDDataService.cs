#region

using RestSharp;

#endregion

namespace SpectrailTestFramework.Services;

public class ICDDataService(RestClient client) : IApiService
{
    public async Task<RestResponse> GetDataAsync(string endpoint)
    {
        var request = new RestRequest(endpoint);
        return await client.ExecuteAsync(request);
    }
}