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
// FileName: IAsyncRepository.cs
// ProjectName: Alstom.Spectrail.Server.Common
// Created by SUBHASISH BISWAS On: 2025-03-04
// Updated by SUBHASISH BISWAS On: 2025-03-21
//  ******************************************************************************/

#endregion

#region

using System.Linq.Expressions;
using Alstom.Spectrail.Server.Common.Attributes;
using Alstom.Spectrail.Server.Common.Entities;

#endregion

namespace Alstom.Spectrail.ICD.Application.Contracts;

public interface IAsyncRepository<T> where T : EntityBase
{
    [RepositoryOperation("GetAll")]
    Task<IEnumerable<T>> GetAllAsync(string? fileName = null, string? sheetName = null);

    [RepositoryOperation("GetById")]
    Task<T> GetByIdAsync(string id);

    [RepositoryOperation("GetByFilter")]
    Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter);

    [RepositoryOperation("Add")]
    Task AddAsync(T entity);

    [RepositoryOperation("Update")]
    Task<bool> UpdateAsync(string id, T entity);

    [RepositoryOperation("Delete")]
    Task<bool> DeleteAsync(string id);

    /// <summary>
    ///     ✅ Adds multiple records efficiently in batch.
    /// </summary>
    [RepositoryOperation("AddMany")]
    Task AddManyAsync(IEnumerable<T> entities);

    [RepositoryOperation("SeedData")]
    Task SeedDataAsync(IEnumerable<T>? entities);

    /// <summary>
    ///     ✅ Deletes all records in the collection.
    /// </summary>
    [RepositoryOperation("DeleteAll")]
    Task<bool> DeleteAllAsync();
}