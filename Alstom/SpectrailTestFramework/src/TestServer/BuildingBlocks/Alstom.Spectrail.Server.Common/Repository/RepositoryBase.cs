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
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.Server.Common.Contracts;
using Alstom.Spectrail.Server.Common.Entities;

#endregion

namespace Alstom.Spectrail.Server.Common.Repository;

public class RepositoryBase(IDataProvider dataProvider) : IAsyncRepository

{
    public async Task<bool> DeleteAsync(string collectionName)
    {
        return await dataProvider.DeleteAsync(collectionName);
    }

    public async Task<bool> DeleteAllAsync()
    {
        return await dataProvider.DeleteAllAsync();
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

    public async Task<IEnumerable<EntityBase>> GetEntityAsync(string fileName, string sheetName)
    {
        return await dataProvider.GetEntityAsync(fileName, sheetName);
    }

    public async Task<Dictionary<string, List<EntityBase>>> GetEntitiesByFileAsync(string fileName)
    {
        return await dataProvider.GetEntitiesByFileAsync(fileName);
    }
}