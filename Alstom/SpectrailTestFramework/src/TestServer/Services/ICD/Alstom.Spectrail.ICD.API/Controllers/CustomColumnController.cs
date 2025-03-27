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
using Alstom.Spectrail.Server.Common.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

#endregion


namespace Alstom.Spectrail.ICD.API.Controllers;

[ApiController]
[Route("api/v1/[Controller]")]
public class CustomColumnController(IMediator mediator, IMapper mapper) : ControllerBase
{
    /// ✅ Fetch GetCustomColumn By Equipment
    [HttpGet("GetCustomColumnByEquipment")]
    public async Task<ActionResult<List<CustomColumnDto>>> GetCustomColumnByEquipment([FromQuery] string fileName,
        [FromQuery] string sheetName)
    {
        if (string.IsNullOrEmpty(fileName)) return BadRequest("❌ File name is required.");
        if (string.IsNullOrEmpty(sheetName)) return BadRequest("❌ Sheet name is required.");

        var data = await mediator.Send(new RepositoryQuery
        {
            FileName = fileName,
            SheetName = sheetName
        });

        if (!data.Any()) return NotFound($"⚠️ No records found for file: {fileName}");
        return Ok(mapper.Map<List<CustomColumnDto>>(data));
    }

    [HttpGet("GetAllCustomColumns")]
    public async Task<IActionResult> GetAllCustomColumns([FromQuery] string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return BadRequest("❌ File name is required.");

        var data = await mediator.Send(new RepositoryQuery { FileName = fileName });

        if (!data.Any()) return NotFound($"⚠️ No records found for file: {fileName}");

        return Ok(data);
    }

    /// ✅ Add a single DCU record (Dynamically calls "AddAsync")
    [HttpPost("RegisterICDFile")]
    public async Task<IActionResult> RegisterICDFile([FromQuery] string filePath)
    {
        var result = await mediator.Send(new RepositoryCommand(RepositoryOperation.Add, new EntityBase()));
        return result ? Ok("✅ Record Added!") : BadRequest("❌ Failed to Add!");
    }

    /// ✅ Delete ICD Files
    [HttpDelete("DeleteICDFile")]
    public async Task<IActionResult> DeleteICDFile([FromQuery] string fileName)
    {
        var result = await mediator.Send(new RepositoryCommand(RepositoryOperation.DeleteAll));
        return result ? Ok("✅ All Records Deleted!") : BadRequest("❌ Failed to Delete!");
    }
}