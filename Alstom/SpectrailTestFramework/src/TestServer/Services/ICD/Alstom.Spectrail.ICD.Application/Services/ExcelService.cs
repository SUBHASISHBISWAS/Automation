/*#region

using System.Security.Cryptography;
using ClosedXML.Excel;
using MediatR;
using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using Alstom.Spectrail.ICD.Domain.Common;

#endregion

namespace Alstom.Spectrail.ICD.Application.Features.ICD.Services;

public class ExcelService(IMediator mediator) : IExcelService
{
    /// <summary>
    ///     ✅ Computes MD5 Checksum for File Change Detection.
    /// </summary>
    public string ComputeFileChecksum(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    /// <summary>
    ///     ✅ Reads Excel, detects changes, and stores in MongoDB dynamically.
    /// </summary>
    public async Task<List<T>> ReadExcelAndStoreAsync<T>(string filePath, string? sheetName = null)
        where T : EntityBase, new()
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"❌ File not found: {filePath}");

        var newChecksum = ComputeFileChecksum(filePath);

        // ✅ Get Repository for the specific entity type
        var repository = mediator.Send(new GetRepositoryQuery<T>()).Result;
        var existingRecords = await repository.GetAllAsync();
        var enumerable = existingRecords as T[] ?? existingRecords.ToArray();
        var entityBases = enumerable.ToList();
        {
            var existingChecksum = entityBases.FirstOrDefault()?.Checksum;

            // ✅ Compare checksum: If unchanged, return stored data
            if (existingChecksum == newChecksum)
            {
                Console.WriteLine($"✅ No changes detected for {typeof(T).Name}. Using cached data.");
                return entityBases.ToList();
            }
        }
        Console.WriteLine($"🔄 Changes detected in {typeof(T).Name}. Updating database...");
        // ✅ Read new data from Excel
        var newRecords = ReadExcel<T>(filePath, sheetName);

        // ✅ Assign Checksum & Prepare for Insertion
        var updatedRecords = newRecords.Select(record =>
        {
            record.Checksum = newChecksum;
            return record;
        }).ToList();

        // ✅ Batch delete & insert operations
        if (enumerable.Length != 0)
        {
            var deleteTasks = enumerable.Select(record => repository.DeleteAsync(record.Id ?? string.Empty));
            await Task.WhenAll(deleteTasks);
        }

        if (updatedRecords.Count == 0) return await Task.FromResult(newRecords);
        {
            var insertTasks = updatedRecords.Select(record => repository.AddAsync(record));
            await Task.WhenAll(insertTasks);
        }

        return await Task.FromResult(newRecords);
    }

    /// <summary>
    ///     ✅ Retrieves stored data from MongoDB.
    /// </summary>
    public async Task<List<T>> GetStoredDataAsync<T>() where T : EntityBase, new()
    {
        var repository = mediator.Send(new GetRepositoryQuery<T>()).Result;
        return (await repository.GetAllAsync()).ToList();
    }

    /// <summary>
    ///     ✅ Reads an Excel file and maps dynamically to a generic entity.
    /// </summary>
    private List<T> ReadExcel<T>(string filePath, string? sheetName = null) where T : EntityBase, new()
    {
        using var workbook = new XLWorkbook(filePath);
        var worksheet = !string.IsNullOrEmpty(sheetName) ? workbook.Worksheet(sheetName) : workbook.Worksheets.First();

        if (worksheet == null)
            throw new Exception("❌ No worksheet found in the Excel file.");

        var rows = worksheet.RowsUsed().Skip(1); // ✅ Skip header row
        var properties = typeof(T).GetProperties();
        var headers = worksheet.Row(1).Cells().Select(c => c.GetString().Trim()).ToList();
        var checksum = ComputeFileChecksum(filePath); // ✅ Precompute checksum

        var entities = new List<T>();

        foreach (var row in rows)
        {
            var entity = new T { Checksum = checksum }; // ✅ Assign checksum

            for (var colIndex = 0; colIndex < headers.Count; colIndex++)
            {
                var header = headers[colIndex].Trim().Replace(" ", "");
                var property =
                    properties.FirstOrDefault(p => p.Name.Equals(header, StringComparison.OrdinalIgnoreCase));

                try
                {
                    var cell = row.Cell(colIndex + 1);
                    var cellValue = ExtractCellValue(cell);

                    if (property != null)
                        property.SetValue(entity, Convert.ChangeType(cellValue, property.PropertyType));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            entities.Add(entity);
        }

        return entities;
    }

    private static object? ExtractCellValue(IXLCell cell)
    {
        if (cell.IsEmpty()) return null;

        return cell.DataType switch
        {
            XLDataType.Text => cell.GetString().Trim(),
            XLDataType.Number => cell.GetDouble(),
            XLDataType.Boolean => cell.GetBoolean(),
            XLDataType.DateTime => cell.GetDateTime(),
            _ => cell.Value
        };
    }
}*/

