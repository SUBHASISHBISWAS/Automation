#region

using System.Security.Cryptography;
using ClosedXML.Excel;
using MediatR;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Application.Features.ICD.Queries.Query;
using SpectrailTestDataProvider.Application.Registry;
using SpectrailTestDataProvider.Application.Utility;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Application.Features.ICD.Services;

public class ICDExcelService(IMediator mediator, ApplicationConfigHelper configHelper) : IExcelService
{
    public async Task<List<T>> ReadExcelAndStoreAsync<T>(string? filePath, string? sheetName)
        where T : EntityBase, new()
    {
        var newChecksum = ComputeFileChecksum(filePath ?? throw new ArgumentNullException(nameof(filePath)));
        var repository = await mediator.Send(new GetRepositoryQuery<T>());
        var existingRecords = await repository.GetAllAsync();
        var entityBases = existingRecords.ToList();
        var existingChecksum = entityBases.FirstOrDefault()?.Checksum;

        if (configHelper.IsFeatureEnabled("EnableChecksumValidation") && existingChecksum == newChecksum)
            return entityBases.ToList();

        var newRecords = ReadExcel<T>(filePath, sheetName ?? throw new ArgumentNullException(nameof(sheetName)));
        newRecords.ForEach(record => record.Checksum = newChecksum);

        if (configHelper.IsFeatureEnabled("EnableBatchProcessing"))
        {
            //await repository.DeleteAllAsync();
            //await repository.AddManyAsync(newRecords);
        }
        else
        {
            foreach (var record in newRecords) await repository.AddAsync(record);
        }

        return newRecords;
    }

    public string ComputeFileChecksum(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    public async Task<List<T>> GetStoredDataAsync<T>() where T : EntityBase, new()
    {
        var repository = mediator.Send(new GetRepositoryQuery<T>()).Result;
        return (await repository.GetAllAsync()).ToList();
    }

    public async Task LoadAllICDsAsync()
    {
        var icdFiles = configHelper.GetICDFiles();
        foreach (var filePath in icdFiles) await ReadAndStoreAllSheetsAsync(filePath);
    }

    private async Task ReadAndStoreAllSheetsAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"❌ File not found: {filePath}");

        using var workbook = new XLWorkbook(filePath);
        foreach (var worksheet in workbook.Worksheets)
        {
            var sheetName = worksheet.Name;
            var entityType = EntityRegistry.GetEntityType(sheetName);

            if (entityType == null)
            {
                Console.WriteLine($"⚠️ No registered entity for sheet: {sheetName}. Skipping...");
                continue;
            }

            var readMethod = typeof(ExcelService).GetMethod(nameof(ReadExcelAndStoreAsync))?
                .MakeGenericMethod(entityType);

            await (Task)readMethod?.Invoke(this, new object[] { filePath, sheetName })!;
        }
    }

    private List<T> ReadExcel<T>(string filePath, string sheetName) where T : EntityBase, new()
    {
        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(sheetName);
        var rows = worksheet.RowsUsed().Skip(1);
        var properties = typeof(T).GetProperties();
        var headers = worksheet.Row(1).Cells().Select(c => c.GetString().Trim()).ToList();
        var entities = new List<T>();

        foreach (var row in rows)
        {
            var entity = new T();
            for (var colIndex = 0; colIndex < headers.Count; colIndex++)
            {
                var property = properties.FirstOrDefault(p =>
                    p.Name.Equals(headers[colIndex].Replace(" ", ""), StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    var cellValue = ExtractCellValue(row.Cell(colIndex + 1));
                    property.SetValue(entity, Convert.ChangeType(cellValue, property.PropertyType));
                }
            }

            entity.Checksum = ComputeFileChecksum(filePath);
            entities.Add(entity);
        }

        return entities;
    }

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