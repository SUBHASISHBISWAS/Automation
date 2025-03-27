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
// FileName: RepositoryBase.cs
// ProjectName: Alstom.Spectrail.Server.Common
// Created by SUBHASISH BISWAS On: 2025-03-04
// Updated by SUBHASISH BISWAS On: 2025-03-27
//  ******************************************************************************/

#endregion

#region

using System.Linq.Expressions;
using Alstom.Spectrail.Server.Common.Contracts;
using Alstom.Spectrail.Server.Common.Entities;

#endregion

namespace Alstom.Spectrail.Server.Common.Repository;

public class RepositoryBase(IDataProvider dataProvider) : IAsyncRepository

{
    public async Task<bool> DeleteAsync(string id)
    {
        return await dataProvider.DeleteAsync(id);
    }

    public async Task<bool> DeleteAllAsync()
    {
        return await dataProvider.DeleteAllAsync();
    }

    /// <summary>
    ///     ✅ Saves a new entity in MongoDB.
    /// </summary>
    public async Task AddAsync(EntityBase entity)
    {
        await dataProvider.AddAsync(entity);
    }

    public async Task<bool> UpdateAsync(string id, EntityBase entity)
    {
        return await dataProvider.UpdateAsync(id, entity);
    }

    public async Task AddManyAsync(IEnumerable<EntityBase> entities)
    {
        await dataProvider.AddManyAsync(entities);
    }

    public async Task SeedDataAsync(IEnumerable<EntityBase>? entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await dataProvider.SeedDataAsync(entities);
    }

    public async Task<EntityBase> GetByIdAsync(string id)
    {
        return await dataProvider.GetByIdAsync(id);
    }

    public async Task<IEnumerable<EntityBase>> GetByFilterAsync(Expression<Func<EntityBase, bool>> filter)
    {
        return await dataProvider.GetByFilterAsync(filter);
    }

    public async Task<IEnumerable<EntityBase>> GetEntityAsync(string fileName, string sheetName)
    {
        return await dataProvider.GetEntityAsync(fileName, sheetName);
    }
}