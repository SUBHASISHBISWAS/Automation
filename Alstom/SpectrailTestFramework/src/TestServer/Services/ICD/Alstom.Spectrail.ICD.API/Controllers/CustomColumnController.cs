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
// Created by SUBHASISH BISWAS On: 2025-03-27
// Updated by SUBHASISH BISWAS On: 2025-03-27
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
    }

    /// ✅ Fetch all Entity records
    [HttpGet("NETWORK-EQUIPMENT")]
    public async Task<ActionResult<List<CustomColumnDto>>> GetEquipmentByFile([FromQuery] string fileName,
        [FromQuery] string sheetName)
    {
        if (string.IsNullOrEmpty(fileName)) return BadRequest("❌ File name is required.");
        if (string.IsNullOrEmpty(sheetName)) return BadRequest("❌ Sheet name is required.");

        var data = await _mediator.Send(new RepositoryQuery
        {
            FileName = fileName,
            SheetName = sheetName
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
}