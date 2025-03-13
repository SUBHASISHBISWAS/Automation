namespace Alstom.Spectrail.ICD.Application.Models;

public class SpectrailMongoDatabaseSettings
{
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }

    public string? CollectionName { get; set; }
}