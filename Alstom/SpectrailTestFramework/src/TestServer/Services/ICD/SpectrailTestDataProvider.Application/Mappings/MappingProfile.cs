#region

using AutoMapper;
using SpectrailTestDataProvider.Application.Features.ICD.Queries.Model;
using SpectrailTestDataProvider.Domain.Entities.ICD;

#endregion

namespace SpectrailTestDataProvider.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DCUEntity, DCUEntityVm>().ReverseMap();
    }
}