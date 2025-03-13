#region

using Alstom.Spectrail.ICD.Domain.Common;

#endregion

namespace Alstom.Spectrail.ICD.Application.Contracts;

public interface IExcelService
{
    Task<List<T>> ReadExcelAndStoreAsync<T>(string filePath, string? sheetName = null) where T : EntityBase, new();
    Task<List<T>> GetStoredDataAsync<T>() where T : EntityBase, new();
    string ComputeFileChecksum(string filePath);

    Task InitializeAsync();
}