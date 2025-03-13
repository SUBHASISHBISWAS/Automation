#region

using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Model;
using AutoMapper;
using Alstom.Spectrail.ICD.Domain.Entities.ICD;

#endregion

namespace Alstom.Spectrail.ICD.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DCUEntity, DCUEntityVm>().ReverseMap();
    }
}