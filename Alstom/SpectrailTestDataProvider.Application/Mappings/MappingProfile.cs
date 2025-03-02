#region

using AutoMapper;
using SpectrailTestDataProvider.Application.Features.ICD_DataProvider.Queries;
using SpectrailTestDataProvider.Domain.Entities;

#endregion

namespace SpectrailTestDataProvider.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ICDEntity, ICDEntityVm>().ReverseMap();
    }
}