#region

using MediatR;
using SpectrailTestDataProvider.Application.Features.ICD_DataProvider.Queries.Model;

#endregion

namespace SpectrailTestDataProvider.Application.Features.ICD_DataProvider.Queries.Query;

public class GetICDQuery(string userName) : IRequest<List<ICDEntityVm>>
{
    public string UserName { get; set; } = userName ?? throw new ArgumentNullException(nameof(userName));
}