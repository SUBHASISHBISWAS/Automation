#region

using MediatR;

#endregion

namespace SpectrailTestDataProvider.Application.Features.ICD_DataProvider.Queries;

public class GetICDQuery(string userName) : IRequest<List<ICDEntityVm>>
{
    public string UserName { get; set; } = userName ?? throw new ArgumentNullException(nameof(userName));
}