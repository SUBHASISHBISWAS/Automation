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
// FileName: IDataProvider.cs
// ProjectName: Alstom.Spectrail.Server.Common
// Created by SUBHASISH BISWAS On: 2025-03-04
// Updated by SUBHASISH BISWAS On: 2025-03-27
//  ******************************************************************************/

#endregion

#region

using System.Linq.Expressions;
using Alstom.Spectrail.Server.Common.Entities;

#endregion

namespace Alstom.Spectrail.Server.Common.Contracts;

public interface IDataProvider
{
    Task<IEnumerable<EntityBase>> GetEntityAsync(string fileName, string sheetName);
    Task<EntityBase> GetByIdAsync(string id);
    Task<IEnumerable<EntityBase>> GetByFilterAsync(Expression<Func<EntityBase, bool>> filter);
    Task AddAsync(EntityBase entity);
    Task<bool> UpdateAsync(string id, EntityBase entity);
    Task<bool> DeleteAsync(string id);

    /// <summary>
    ///     ✅ Adds multiple records efficiently in batch.
    /// </summary>
    Task AddManyAsync(IEnumerable<EntityBase> entities);


    Task SeedDataAsync(IEnumerable<EntityBase> entities);

    /// <summary>
    ///     ✅ Deletes all records in the collection.
    /// </summary>
    Task<bool> DeleteAllAsync();
}