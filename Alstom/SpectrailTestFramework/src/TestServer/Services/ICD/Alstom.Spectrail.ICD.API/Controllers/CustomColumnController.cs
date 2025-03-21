#region ©COPYRIGHT

// /*******************************************************************************
//  *   © COPYRIGHT ALSTOM SA 2025 - ALL RIGHTS RESERVED
//  *
//  *  This file is part of the Alstom Spectrail project.
//  *  Unauthorized copying, modification, distribution, or use of this file,
//  *  via any medium, is strictly prohibited.
//  *
//  *  Proprietary and confidential.
//  *  Created and maintained by Alstom for internal use in the Spectrail project.
//  ******************************************************************************/
// 
//  /*******************************************************************************
// AuthorName: SUBHASISH BISWAS
// Email: subhasish.biswas@alstomgroup.com
// FileName: CustomColumnController.cs
// ProjectName: Alstom.Spectrail.ICD.API
// Created by SUBHASISH BISWAS On: 2025-03-21
// Updated by SUBHASISH BISWAS On: 2025-03-22
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Enums;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using Alstom.Spectrail.ICD.Domain.DTO.ICD;
using Alstom.Spectrail.ICD.Domain.Entities.ICD;
using Alstom.Spectrail.Server.Common.Configuration;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

#endregion


namespace Alstom.Spectrail.ICD.API.Controllers;

[ApiController]
[Route("api/v1/[Controller]")]
public class CustomColumnController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CustomColumnController(IMediator mediator, IServerConfigHelper configHelper, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
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
    [HttpGet("DCU")]
    public async Task<ActionResult<List<CustomColumnDto>>> GetDCU([FromQuery] string? fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return BadRequest("❌ File name is required.");

        var data = await _mediator.Send(new RepositoryQuery
        {
            FileName = fileName,
            SheetName = "bce"
        });

        if (!data.Any()) return NotFound($"⚠️ No records found for file: {fileName}");
        return Ok(_mapper.Map<List<CustomColumnDto>>(data));
    }

    [HttpGet("AllDCURecords")]
    public async Task<IActionResult> GetAllDCURecords([FromQuery] string? fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return BadRequest("❌ File name is required.");

        var data = await _mediator.Send(new RepositoryQuery { FileName = fileName });

        if (!data.Any()) return NotFound($"⚠️ No records found for file: {fileName}");

        return Ok(data);
    }

    /// ✅ Fetch a specific DCU record by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDCURecordById(string id)
    {
        var data = await _mediator.Send(new RepositoryQuery());
        return Ok(data);
    }

    /// ✅ Add a single DCU record (Dynamically calls "AddAsync")
    [HttpPost("add")]
    public async Task<IActionResult> AddDCURecord([FromBody] DCUEntity entity)
    {
        var result = await _mediator.Send(new RepositoryCommand(RepositoryOperation.Add, entity));
        return result ? Ok("✅ Record Added!") : BadRequest("❌ Failed to Add!");
    }

    /// ✅ Add multiple DCU records (Dynamically calls "InitializeAsync")
    [HttpPost("add-many")]
    public async Task<IActionResult> AddManyDCURecords([FromBody] List<DCUEntity> entities)
    {
        var result =
            await _mediator.Send(new RepositoryCommand(RepositoryOperation.AddMany, entities: entities));
        return result ? Ok("✅ Records Added!") : BadRequest("❌ Failed to Add!");
    }

    /// ✅ Update a DCU record (Dynamically calls "UpdateAsync")
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateDCURecord(string id, [FromBody] DCUEntity entity)
    {
        var result = await _mediator.Send(new RepositoryCommand(RepositoryOperation.Update, entity, id: id));
        return result ? Ok("✅ Record Updated!") : BadRequest("❌ Failed to Update!");
    }

    /// ✅ Delete a DCU record (Dynamically calls "DeleteAsync")
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteDCURecord(string id)
    {
        var result = await _mediator.Send(new RepositoryCommand(RepositoryOperation.Delete, id: id));
        return result ? Ok("✅ Record Deleted!") : BadRequest("❌ Failed to Delete!");
    }

    /// ✅ Delete all DCU records (Dynamically calls "DeleteAllAsync")
    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllDCURecords()
    {
        var result = await _mediator.Send(new RepositoryCommand(RepositoryOperation.DeleteAll));
        return result ? Ok("✅ All Records Deleted!") : BadRequest("❌ Failed to Delete!");
    }

    /*private async Task<List<EntityBase>> FetchEntitiesDynamicallyAsync(string entityName, string fileName)
    {
        var entityType = EntityRegistry.GetEntityType(entityName);
        if (entityType == null)
            throw new InvalidOperationException($"❌ Entity type '{entityName}' could not be resolved.");

        var queryType = typeof(RepositoryQuery<>).MakeGenericType(entityType);
        var queryInstance = Activator.CreateInstance(queryType);
        queryType.GetProperty("FileName")?.SetValue(queryInstance, fileName);

        var sendMethod = typeof(IMediator)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(m => m.Name == "Send" && m.IsGenericMethod)
            .Where(m =>
            {
                var parameters = m.GetParameters();
                return parameters.Length == 2 &&
                       parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IRequest<>) &&
                       parameters[1].ParameterType == typeof(CancellationToken);
            })
            .FirstOrDefault();

        if (sendMethod == null)
            throw new InvalidOperationException("❌ Could not find a matching generic Send method on IMediator.");

        var genericSend = sendMethod.MakeGenericMethod(typeof(List<EntityBase>));
        var cancellationToken = CancellationToken.None;

        var task = (Task)genericSend.Invoke(_mediator, new[] { queryInstance, cancellationToken });
        await task.ConfigureAwait(false);

        var result = task.GetType().GetProperty("Result")?.GetValue(task) as List<EntityBase>;
        return result ?? new List<EntityBase>();
    }*/
}