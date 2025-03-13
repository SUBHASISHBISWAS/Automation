#region

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Alstom.Spectrail.ICD.Application.Enums;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using Alstom.Spectrail.ICD.Domain.Entities.ICD;
using SpectrailTestDataProvider.Shared.Configuration;

#endregion

namespace Alstom.Spectrail.ICD.API.Controllers;

[ApiController]
[Route("api/v1/[Controller]")]
public class ICDController : ControllerBase
{
    private readonly IMediator _mediator;

    public ICDController(IMediator mediator, ServerConfigHelper configHelper)
    {
        _mediator = mediator;
        if (configHelper.IsFeatureEnabled("EnableEagerLoading"))
            Task.Run(async () =>
            {
                var success = await _mediator.Send(new SeedICDDataCommand());
                return success
                    ? Ok("✅ ICD Database Seeded Successfully!")
                    : StatusCode(500, "⚠️ ICD Database Seeding Failed!");
            });
    }

    /// ✅ Fetch all DCU records
    [HttpGet("all")]
    public async Task<IActionResult> GetAllDCURecords()
    {
        var data = await _mediator.Send(new RepositoryQuery<DCUEntity>());
        return Ok(data);
    }

    /// ✅ Fetch a specific DCU record by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDCURecordById(string id)
    {
        var data = await _mediator.Send(new RepositoryQuery<DCUEntity>(id));
        return Ok(data);
    }

    /// ✅ Add a single DCU record (Dynamically calls "AddAsync")
    [HttpPost("add")]
    public async Task<IActionResult> AddDCURecord([FromBody] DCUEntity entity)
    {
        var result = await _mediator.Send(new RepositoryCommand<DCUEntity>(RepositoryOperation.Add, entity));
        return result ? Ok("✅ Record Added!") : BadRequest("❌ Failed to Add!");
    }

    /// ✅ Add multiple DCU records (Dynamically calls "InitializeAsync")
    [HttpPost("add-many")]
    public async Task<IActionResult> AddManyDCURecords([FromBody] List<DCUEntity> entities)
    {
        var result =
            await _mediator.Send(new RepositoryCommand<DCUEntity>(RepositoryOperation.AddMany, entities: entities));
        return result ? Ok("✅ Records Added!") : BadRequest("❌ Failed to Add!");
    }

    /// ✅ Update a DCU record (Dynamically calls "UpdateAsync")
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateDCURecord(string id, [FromBody] DCUEntity entity)
    {
        var result = await _mediator.Send(new RepositoryCommand<DCUEntity>(RepositoryOperation.Update, entity, id: id));
        return result ? Ok("✅ Record Updated!") : BadRequest("❌ Failed to Update!");
    }

    /// ✅ Delete a DCU record (Dynamically calls "DeleteAsync")
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteDCURecord(string id)
    {
        var result = await _mediator.Send(new RepositoryCommand<DCUEntity>(RepositoryOperation.Delete, id: id));
        return result ? Ok("✅ Record Deleted!") : BadRequest("❌ Failed to Delete!");
    }

    /// ✅ Delete all DCU records (Dynamically calls "DeleteAllAsync")
    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllDCURecords()
    {
        var result = await _mediator.Send(new RepositoryCommand<DCUEntity>(RepositoryOperation.DeleteAll));
        return result ? Ok("✅ All Records Deleted!") : BadRequest("❌ Failed to Delete!");
    }
}