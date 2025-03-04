namespace SpectrailTestDataProvider.Application.Features.ICD_DataProvider.Queries.Model;

public class ICDEntityVm
{
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? ICDName { get; set; }
    public string? ICDDescription { get; set; }
}