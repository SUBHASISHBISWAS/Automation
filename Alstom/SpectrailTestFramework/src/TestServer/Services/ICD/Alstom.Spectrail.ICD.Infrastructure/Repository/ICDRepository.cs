#region ©COPYRIGHT

// /*******************************************************************************
//  *   © COPYRIGHT ALSTOM SA 2025 - ALL RIGHTS RESERVED
//  *
//  *  This file is part of the Alstom Spectrail project.
//  *  Unauthorized copying, modification, distribution, or use of this file,
//  *  via any medium, is strictly prohibited.
//  *
//  *  Proprietary and confidential.
//  *  Created and maintained by Alstom for internal use in the Spectrail project.
//  ******************************************************************************/
// 
//  /*******************************************************************************
// AuthorName: SUBHASISH BISWAS
// Email: subhasish.biswas@alstomgroup.com
// FileName: ICDRepository.cs
// ProjectName: Alstom.Spectrail.ICD.Infrastructure
// Created by SUBHASISH BISWAS On: 2025-03-04
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.Server.Common.Entities;
using Alstom.Spectrail.Server.Common.Repository;

#endregion

namespace Alstom.Spectrail.ICD.Infrastructure.Repository;

public class ICDRepository<T>(IDataProvider<T> dataProvider)
    : RepositoryBase<T>(dataProvider), IICDRepository<T> where T : EntityBase
{
}