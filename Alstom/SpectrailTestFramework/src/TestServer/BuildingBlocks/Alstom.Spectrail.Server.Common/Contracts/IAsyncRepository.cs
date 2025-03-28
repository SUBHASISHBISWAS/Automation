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
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.Server.Common.Attributes;
using Alstom.Spectrail.Server.Common.Entities;

#endregion

namespace Alstom.Spectrail.Server.Common.Contracts;

public interface IAsyncRepository
{
    [RepositoryOperation("GetEntity")]
    Task<IEnumerable<EntityBase>> GetEntityAsync(string fileName, string sheetName);

    [RepositoryOperation("GetEntitiesByFile")]
    Task<Dictionary<string, List<EntityBase>>> GetEntitiesByFileAsync(string fileName);

    [RepositoryOperation("Update")]
    Task<bool> UpdateAsync(string id, EntityBase entity);

    [RepositoryOperation("Delete")]
    Task<bool> DeleteAsync(string collectionName);

    /// <summary>
    ///     ✅ Adds multiple records efficiently in batch.
    /// </summary>
    [RepositoryOperation("AddMany")]
    Task AddManyAsync(IEnumerable<EntityBase> entities);

    [RepositoryOperation("SeedData")]
    Task SeedDataAsync(IEnumerable<EntityBase>? entities);

    /// <summary>
    ///     ✅ Deletes all records in the collection.
    /// </summary>
    [RepositoryOperation("DeleteAll")]
    Task<bool> DeleteAllAsync();
}