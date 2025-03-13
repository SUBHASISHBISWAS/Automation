#region

using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Model;
using AutoMapper;
using SpectrailTestDataProvider.Domain.Entities.ICD;

#endregion

namespace Alstom.Spectrail.ICD.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DCUEntity, DCUEntityVm>().ReverseMap();
    }
}