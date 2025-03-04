#region

using SpectrailTestDataProvider.Domain.Common;

// ReSharper disable ClassNeverInstantiated.Global

#endregion

namespace SpectrailTestDataProvider.Domain.Entities;

public class ICDEntity : EntityBase
{
    public ICDEntity()
    {
        CreatedDate = DateTime.UtcNow;
        LastModifiedDate = DateTime.UtcNow;
        CreatedBy = "SUBHASISH";
        LastModifiedBy = "SUBHASISH";
    }

    public string? ICDName { get; set; }
    public string? ICDDescription { get; set; }
}