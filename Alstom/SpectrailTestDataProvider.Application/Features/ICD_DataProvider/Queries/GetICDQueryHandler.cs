#region

using AutoMapper;
using MediatR;
using SpectrailTestDataProvider.Application.Contracts;

#endregion

namespace SpectrailTestDataProvider.Application.Features.ICD_DataProvider.Queries;

public class GetICDQueryHandler(IICDRepository icdRepository, IMapper mapper)
    : IRequestHandler<GetICDQuery, List<ICDEntityVm>>
{
    private readonly IICDRepository _icdRepository =
        icdRepository ?? throw new ArgumentNullException(nameof(icdRepository));

    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<List<ICDEntityVm>> Handle(GetICDQuery request, CancellationToken cancellationToken)
    {
        var orderList = await _icdRepository.GetAllAsync();
        return _mapper.Map<List<ICDEntityVm>>(orderList);
    }
}