#region

using SpectrailTestDataProvider.Domain.Common;

// ReSharper disable ClassNeverInstantiated.Global

#endregion

namespace SpectrailTestDataProvider.Domain.Entities;

public class ICDEntity : EntityBase
{
    public string? ICDName { get; set; } = "Spectrail-ICD";
    public string? ICDDescription { get; set; } = "SUBHASISH";
}