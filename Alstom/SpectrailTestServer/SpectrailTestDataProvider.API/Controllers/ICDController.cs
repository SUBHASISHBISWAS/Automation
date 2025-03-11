#region

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpectrailTestDataProvider.API.Utility;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Application.Features.ICD.Queries.Model;
using SpectrailTestDataProvider.Application.Features.ICD.Queries.Query;
using SpectrailTestDataProvider.Domain.Entities.ICD;

#endregion

namespace SpectrailTestDataProvider.API.Controllers;

[Route("api/v1/[Controller]")]
[ApiController]
public class ICDController(IMediator mediator, IExcelService excelService, IMapper mapper,ServerConfigHelper configHelper) : 
    ControllerBase
{
    /// <summary>
    ///     ✅ Fetch all DCU records from MongoDB
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<DCUEntityVm>>> GetAllDCURecords()
    {
        var repository = await mediator.Send(new GetRepositoryQuery<DCUEntity>());
        var data = await repository.GetAllAsync();
        return mapper.Map<List<DCUEntityVm>>(data);
        return Ok(data);
    }

    /// <summary>
    ///     ✅ Reads an Excel file and stores DCU data in MongoDB (Detects Changes)
    /// </summary>
    [HttpPost("import-excel")]
    public async Task<ActionResult<List<DCUEntityVm>>> ImportDCUDataFromExcel(
        [FromQuery] string? filePath ="" ,
        [FromQuery] string? sheetName = "DCU")
    {
        if (string.IsNullOrEmpty(filePath))
            filePath = configHelper.GetSetting("ICD_URL-1");
        if (string.IsNullOrEmpty(filePath))
            sheetName = "DCU";
        
        var data = await excelService.ReadExcelAndStoreAsync<DCUEntity>(filePath!, sheetName);
        return mapper.Map<List<DCUEntityVm>>(data);
        return Ok(data);
    }

    /// <summary>
    ///     ✅ Fetch a single DCU record by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DCUEntityVm>> GetDCURecordById(string id)
    {
        var repository = await mediator.Send(new GetRepositoryQuery<DCUEntity>());
        var entity = await repository.GetByIdAsync(id);
        return mapper.Map<DCUEntityVm>(entity);
        return Ok(entity);
    }
}