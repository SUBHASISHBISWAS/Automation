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
// FileName: ICDExcelService.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-22
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

#region

using System.Security.Cryptography;
using System.Text;
using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Enums;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using Alstom.Spectrail.ICD.Application.Registry;
using Alstom.Spectrail.ICD.Application.Utility;
using Alstom.Spectrail.Server.Common.Configuration;
using Alstom.Spectrail.Server.Common.Entities;
using ClosedXML.Excel;
using MediatR;

#endregion

namespace Alstom.Spectrail.ICD.Application.Services;

/// <summary>
///     ✅ Manages dynamic reading, checksum validation, and MongoDB storage of Excel data.
/// </summary>
public class ICDExcelService(IMediator mediator, IServerConfigHelper configHelper) : IExcelService
{
    /// <summary>
    ///     ✅ Initializes all configured ICD files from `appsettings.json`
    /// </summary>
    public async Task InitializeAsync(List<string> icdFiles)
    {
        foreach (var filePath in icdFiles)
            try
            {
                await ReadAndStoreAllSheetsAsync(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing {filePath}: {ex.Message}");
            }
    }


    /// <summary>
    ///     ✅ Reads and processes all sheets dynamically.
    /// </summary>
    private async Task ReadAndStoreAllSheetsAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"❌ File not found: {filePath}");

        var sheetNames = (await EntityRegistry.GetRegisteredEquipmentMappingsByFile(filePath))
            .Select(x => x.SheetName)
            .ToList();
        var fileName = filePath.GetFileNameWithoutExtension();

        var worksheets = EntityRegistry.RegisteredWorksheets.IsEmpty
            ? new XLWorkbook(filePath).Worksheets
                .Where(ws =>
                    sheetNames.Select(s => s.Trim().Replace(" ", "").ToLower())
                        .Contains(ws.Name.Trim().Replace(" ", "").ToLower()))
                .ToList()
            : EntityRegistry.RegisteredWorksheets[fileName];

        foreach (var worksheet in worksheets)
        {
            var sheetName = worksheet.Name.Trim().Replace(" ", "").ToLower();
            var entityType = EntityRegistry.GetEntityType(sheetName, fileName);

            if (entityType == null)
            {
                Console.WriteLine($"⚠️ No registered entity for sheet: {sheetName}. Skipping...");
                continue;
            }

            Console.WriteLine($"✅ Using entity type: {entityType.FullName} for '{filePath}:{sheetName}'");

            var uniqueKey = $"{fileName}_{sheetName}";
            var newFileChecksum = filePath.ComputeFileHash();

            var existingRecords = await mediator.Send(new RepositoryQuery
            {
                FileName = fileName,
                SheetName = sheetName
            });

            var existingMap = existingRecords.ToDictionary(
                e => e.Id.ToString(),
                e => e.RowChecksum,
                StringComparer.OrdinalIgnoreCase);

            var newRecords = ReadExcel(entityType, worksheet);
            foreach (var record in newRecords)
            {
                record.FileKey = uniqueKey;
                record.FileName = fileName;
                record.SheetName = sheetName;
                record.FileChecksum = newFileChecksum;
                record.RowChecksum = ComputeChecksumFromProperties(record);
            }

            var changedOrNew = newRecords
                .Where(nr =>
                    !existingMap.TryGetValue(nr.Id.ToString(), out var checksum) ||
                    checksum != nr.RowChecksum)
                .ToList();

            if (changedOrNew.Count > 0)
            {
                var eagerLoad = configHelper.IsFeatureEnabled("EnableEagerLoading") ||
                                configHelper.IsFeatureEnabled("EnableMiddlewarePreloading");

                await ExecuteRepositoryCommand(eagerLoad, changedOrNew);
                Console.WriteLine($"✅ Stored {changedOrNew.Count} updated/new records from {uniqueKey}.");
            }
            else
            {
                Console.WriteLine($"✅ No changes detected for {uniqueKey}. Skipping storage.");
            }
        }
    }

    /// <summary>
    ///     ✅ Reads an Excel file and maps dynamically to a strongly typed entity.
    /// </summary>
    private static List<EntityBase> ReadExcel(Type entityType, IXLWorksheet worksheet)
    {
        var rows = worksheet.RowsUsed().Skip(1); // ✅ Skip header row
        var properties = entityType.GetProperties();
        var headers = worksheet.Row(1).Cells().Select(c => c.GetString().Trim().Replace(" ", "")).ToList();

        var entities = new List<EntityBase>();

        foreach (var row in rows)
        {
            var entity = Activator.CreateInstance(entityType) as EntityBase;
            for (var colIndex = 0; colIndex < headers.Count; colIndex++)
            {
                var property = properties.FirstOrDefault(p =>
                    p.Name.Equals(headers[colIndex], StringComparison.OrdinalIgnoreCase));

                if (property == null) continue;
                try
                {
                    var cellValue = ExtractCellValue(row.Cell(colIndex + 1));
                    if (cellValue != null)
                    {
                        var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        var sanitizedValue = cellValue.ToString()?.Trim();

                        if (!string.IsNullOrEmpty(sanitizedValue))
                            property.SetValue(entity, Convert.ChangeType(sanitizedValue, targetType));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"⚠️ Mapping error: Column '{headers[colIndex]}' → Property '{property.Name}': {ex.Message}");
                }
            }


            entities.Add(entity ?? throw new InvalidOperationException());
        }

        return entities;
    }

    /// <summary>
    ///     ✅ Executes repository operations dynamically.
    /// </summary>
    private async Task ExecuteRepositoryCommand<T>(bool isEagerLoading, List<T> newRecords) where T : EntityBase
    {
        if (isEagerLoading)
            await mediator.Send(new RepositoryCommand(RepositoryOperation.SeedData, newRecords));
    }

    private static string ComputeChecksumFromProperties(object obj)
    {
        var props = obj.GetType().GetProperties()
            .Where(p => p.CanRead && p.Name != "RowChecksum")
            .OrderBy(p => p.Name)
            .Select(p => $"{p.Name}:{p.GetValue(obj)?.ToString()?.Trim()}");

        var concat = string.Join("|", props);
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(concat));
        return Convert.ToHexString(bytes);
    }

    /// <summary>
    ///     ✅ Extracts and normalizes Excel cell values.
    /// </summary>
    private static object? ExtractCellValue(IXLCell cell)
    {
        return cell.DataType switch
        {
            XLDataType.Text => cell.GetString().Trim(),
            XLDataType.Number => cell.GetDouble(),
            XLDataType.Boolean => cell.GetBoolean(),
            XLDataType.DateTime => cell.GetDateTime(),
            _ => cell.Value
        };
    }
}