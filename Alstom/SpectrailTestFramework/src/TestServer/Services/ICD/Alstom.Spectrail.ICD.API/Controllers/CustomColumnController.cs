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
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Enums;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using Alstom.Spectrail.ICD.Application.Features.ResetServer.Commands.Command;
using Alstom.Spectrail.ICD.Domain.DTO.ICD;
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
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error retrieving custom columns: {ex.Message}");
            return StatusCode(500, "An unexpected error occurred while retrieving custom columns.");
        }
    }

    [HttpGet("GetAllCustomColumns")]
    public async Task<IActionResult> GetAllCustomColumns([FromQuery] string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return BadRequest("❌ File name is required.");
        try
        {
            var data = await mediator.Send(new GetEntitiesByFileCommand(fileName));

            if (!data.Any()) return NotFound($"⚠️ No records found for file: {fileName}");

            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error retrieving custom columns: {ex.Message}");
            return StatusCode(500, "An unexpected error occurred while retrieving custom columns.");
        }
    }

    /// ✅ Add a single DCU record (Dynamically calls "AddAsync")
    [HttpPost("RegisterICDFile")]
    public async Task<IActionResult> RegisterICDFile([FromQuery] string filePath)
    {
        try
        {
            var result = await mediator.Send(new RepositoryCommand(RepositoryOperation.Add, fileName: filePath));
            return result ? Ok("✅ Record Added!") : BadRequest("❌ Failed to Add!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error retrieving custom columns: {ex.Message}");
            return StatusCode(500, "An unexpected error occurred while retrieving custom columns.");
        }
    }

    /// ✅ Delete ICD Files
    [HttpDelete("DeleteICDRecord")]
    public async Task<IActionResult> DeleteICDRecord([FromQuery] string fileName)
    {
        try
        {
            var result = await mediator.Send(new RepositoryCommand(RepositoryOperation.Delete, fileName: fileName));
            return result ? Ok("✅ All Records Deleted!") : BadRequest("❌ Failed to Delete!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error retrieving custom columns: {ex.Message}");
            return StatusCode(500, "An unexpected error occurred while retrieving custom columns.");
        }
    }

    [HttpDelete("DeleteAllICDRecord")]
    public async Task<IActionResult> DeleteAllICDRecord([FromQuery] string fileName)
    {
        try
        {
            var result = await mediator.Send(new RepositoryCommand(RepositoryOperation.DeleteAll));
            return result ? Ok("✅ All Records Deleted!") : BadRequest("❌ Failed to Delete!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error retrieving custom columns: {ex.Message}");
            return StatusCode(500, "An unexpected error occurred while retrieving custom columns.");
        }
    }

    [HttpPost("ResetServer")]
    public async Task<IActionResult> ResetServer()
    {
        try
        {
            ResetSpectrailServerCommand command = new();

            var result = await mediator.Send(command);

            return result
                ? Ok("✅ Spectrail server has been successfully reset.")
                : StatusCode(500, "❌ Failed to reset the Spectrail server.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception in ResetServer endpoint: {ex.Message}");
            return StatusCode(500, $"❌ Internal server error: {ex.Message}");
        }
    }
}