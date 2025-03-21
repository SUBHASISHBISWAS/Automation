/*#region ©COPYRIGHT

// /*******************************************************************************
//  *   © COPYRIGHT ALSTOM SA 2025 - ALL RIGHTS RESERVED
//  *
//  *  This file is part of the Alstom Spectrail project.
//  *  Unauthorized copying, modification, distribution, or use of this file,
//  *  via any medium, is strictly prohibited.
//  *
//  *  Proprietary and confidential.
//  *  Created and maintained by Alstom for internal use in the Spectrail project.
//  *****************************************************************************#1#
//
//  /*******************************************************************************
// AuthorName: SUBHASISH BISWAS
// Email: subhasish.biswas@alstomgroup.com
// FileName: DynamicMappingProfile.cs
// ProjectName: Alstom.Spectrail.ICD.API
// Created by SUBHASISH BISWAS On: 2025-03-21
// Updated by SUBHASISH BISWAS On: 2025-03-21
//  *****************************************************************************#1#

#endregion

#region

using Alstom.Spectrail.ICD.Domain.DTO.ICD;
using AutoMapper;

#endregion

public class DynamicMappingProfile : Profile
{
    public DynamicMappingProfile(IEnumerable<Type> dynamicEntityTypes)
    {
        foreach (var type in dynamicEntityTypes)
        {
            var dtoType = typeof(DCUDto); // replace with logic to resolve correct DTO

            CreateMap(type, dtoType).ReverseMap();
        }
    }
}*/

