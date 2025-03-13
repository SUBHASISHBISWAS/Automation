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
// Created by SUBHASISH BISWAS On: 2025-03-12
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using System.Reflection;
using System.Security.Cryptography;
using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Enums;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using Alstom.Spectrail.ICD.Application.Registry;
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
    ///     ✅ Reads an Excel file, detects changes, and updates MongoDB.
    /// </summary>
    public async Task<List<T>> ReadExcelAndStoreAsync<T>(string? filePath, string? sheetName)
        where T : EntityBase, new()
    {
        if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(sheetName))
            throw new ArgumentNullException("❌ File path or sheet name cannot be null!");

        var newChecksum = ComputeFileChecksum(filePath);

        // ✅ Fetch existing data
        var existingRecords = await mediator.Send(new RepositoryQuery<T>());
        var entityBases = existingRecords.ToList();
        var existingChecksum = entityBases.FirstOrDefault()?.Checksum;

        // ✅ Skip processing if checksum validation is enabled and file is unchanged
        if (configHelper.IsFeatureEnabled("EnableChecksumValidation") && existingChecksum == newChecksum)
        {
            Console.WriteLine($"✅ No changes detected for {sheetName}. Using existing data.");
            return entityBases.ToList();
        }

        var newRecords = ReadExcel<T>(filePath, sheetName);
        newRecords.ForEach(record => record.Checksum = newChecksum);

        // ✅ Use `RepositoryCommand<T>` for efficient batch operations
        var isFeatureEnabled = configHelper.IsFeatureEnabled("EnableEagerLoading") ||
                               configHelper.IsFeatureEnabled("EnableMiddlewarePreloading");
        await ExecuteRepositoryCommand(isFeatureEnabled, newRecords);

        Console.WriteLine($"✅ Successfully processed {newRecords.Count} records from {sheetName}.");
        return newRecords;
    }

    /// <summary>
    ///     ✅ Retrieves stored data from MongoDB using RepositoryQuery.
    /// </summary>
    public async Task<List<T>> GetStoredDataAsync<T>() where T : EntityBase, new()
    {
        return (await mediator.Send(new RepositoryQuery<T>())).ToList();
    }

    /// <summary>
    ///     ✅ Computes MD5 checksum for file change detection.
    /// </summary>
    public string ComputeFileChecksum(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    /// <summary>
    ///     ✅ Initializes all configured ICD files from `appsettings.json`
    /// </summary>
    public async Task InitializeAsync()
    {
        var icdFiles = configHelper.GetICDFiles();
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
    ///     ✅ Executes repository operations dynamically.
    /// </summary>
    private async Task ExecuteRepositoryCommand<T>(bool isEagerLoading, List<T> newRecords) where T : EntityBase
    {
        if (isEagerLoading)
        {
            await mediator.Send(new RepositoryCommand<T>(RepositoryOperation.DeleteAll));
            await mediator.Send(new RepositoryCommand<T>(RepositoryOperation.SeedData, entities: newRecords));
        }
        else
        {
            foreach (var record in newRecords)
                await mediator.Send(new RepositoryCommand<T>(RepositoryOperation.Add, record));
        }
    }

    /// <summary>
    ///     ✅ Reads and processes all sheets dynamically.
    /// </summary>
    private async Task ReadAndStoreAllSheetsAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"❌ File not found: {filePath}");

        using var workbook = new XLWorkbook(filePath);
        foreach (var worksheet in workbook.Worksheets)
        {
            var sheetName = worksheet.Name;
            var entityType = EntityRegistry.GetEntityType(sheetName);

            if (entityType == null || !typeof(EntityBase).IsAssignableFrom(entityType))
            {
                Console.WriteLine($"⚠️ No valid entity mapped for sheet: {sheetName}. Skipping...");
                continue;
            }

            // ✅ Ensure entity type is properly instantiated before proceeding
            try
            {
                await InvokeGenericMethod(nameof(ReadExcelAndStoreAsync), entityType, filePath, sheetName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing sheet '{sheetName}': {ex.Message}");
            }
        }
    }

    /// <summary>
    ///     ✅ Dynamically invokes a generic method.
    /// </summary>
    private async Task InvokeGenericMethod(string methodName, Type entityType, params object[] parameters)
    {
        var method = typeof(ICDExcelService)
            .GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public)?
            .MakeGenericMethod(entityType);

        if (method == null)
            throw new InvalidOperationException($"❌ Method '{methodName}' not found in {nameof(ICDExcelService)}.");

        await (Task)method.Invoke(this, parameters)!;
    }

    /// <summary>
    ///     ✅ Reads an Excel file and maps dynamically to a strongly typed entity.
    /// </summary>
    private List<T> ReadExcel<T>(string filePath, string sheetName) where T : EntityBase, new()
    {
        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(sheetName);
        var rows = worksheet.RowsUsed().Skip(1); // ✅ Skip header row
        var properties = typeof(T).GetProperties();
        var headers = worksheet.Row(1).Cells().Select(c => c.GetString().Trim().Replace(" ", "")).ToList();

        var entities = new List<T>();

        foreach (var row in rows)
        {
            var entity = new T();
            for (var colIndex = 0; colIndex < headers.Count; colIndex++)
            {
                var property = properties.FirstOrDefault(p =>
                    p.Name.Equals(headers[colIndex], StringComparison.OrdinalIgnoreCase));

                if (property == null) continue;
                try
                {
                    var cellValue = ExtractCellValue(row.Cell(colIndex + 1));
                    if (cellValue != null)
                        try
                        {
                            var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                            // ✅ Normalize cell value
                            var sanitizedValue = cellValue.ToString()?.Trim();

                            // ✅ Skip conversion if value is empty
                            if (string.IsNullOrEmpty(sanitizedValue)) continue;

                            // ✅ Handle Enum values safely
                            if (property.PropertyType.IsEnum)
                            {
                                if (Enum.TryParse(property.PropertyType, sanitizedValue, true, out var enumValue))
                                    property.SetValue(entity, enumValue);
                                else
                                    Console.WriteLine(
                                        $"⚠️ Invalid enum value '{sanitizedValue}' for property '{property.Name}'.");
                            }
                            else
                            {
                                property.SetValue(entity, Convert.ChangeType(sanitizedValue, targetType));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                $"⚠️ Error converting column '{headers[colIndex]}' value '{cellValue}' to property '{property.Name}': {ex.Message}");
                        }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"⚠️ Error mapping column '{headers[colIndex]}' to property '{property.Name}': {ex.Message}");
                }
            }

            entity.Checksum = ComputeFileChecksum(filePath);
            entities.Add(entity);
        }

        return entities;
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