#region

using RestSharp;

#endregion

namespace Alstom.Spectrail.TestFramework.API.APIClient;

public class ApiClient(string baseUrl)
{
    private readonly RestClient _client = new(baseUrl);

    public async Task<RestResponse> ExecuteAsync(RestRequest request)
    {
        return await _client.ExecuteAsync(request);
    }
}