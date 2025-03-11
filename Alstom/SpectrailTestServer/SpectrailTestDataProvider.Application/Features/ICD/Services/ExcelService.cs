#region

using System.Security.Cryptography;
using ClosedXML.Excel;
using MediatR;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Application.Features.ICD.Queries.Query;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Application.Features.ICD.Services;

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
        var entityBases = existingRecords.ToList();
        {
            var existingChecksum = entityBases.FirstOrDefault()?.Checksum;

            // ✅ Compare checksum: If unchanged, return stored data
            if (existingChecksum == newChecksum) return entityBases.ToList();
        }

        // ✅ Read new data from Excel
        var newRecords = ReadExcel<T>(filePath, sheetName);

        // ✅ Store in MongoDB (Delete old & Insert new)
        foreach (var record in entityBases) await repository.DeleteAsync(record.Id);
        foreach (var record in newRecords) await repository.AddAsync(record);

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

        var entities = new List<T>();

        foreach (var row in rows)
        {
            var entity = new T();

            for (var colIndex = 0; colIndex < headers.Count; colIndex++)
            {
                var header = headers[colIndex];
                var normalizedHeader = header.Trim().Replace(" ", ""); // Remove spaces from the header
                var property = properties.FirstOrDefault(p =>
                    p.Name.Equals(normalizedHeader, StringComparison.OrdinalIgnoreCase));

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

            entity.Checksum = ComputeFileChecksum(filePath);
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
}