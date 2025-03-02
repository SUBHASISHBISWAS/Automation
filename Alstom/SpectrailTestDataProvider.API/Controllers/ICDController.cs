#region

using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpectrailTestDataProvider.Application.Features.ICD_DataProvider.Queries.Model;
using SpectrailTestDataProvider.Application.Features.ICD_DataProvider.Queries.Query;

#endregion

namespace SpectrailTestDataProvider.API.Controllers;

[ApiController]
[Route("api/v1/[Controller]")]
public class ICDController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet("{userName}", Name = "GetOrder")]
    [ProducesResponseType(typeof(IEnumerable<ICDEntityVm>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<ICDEntityVm>>> GetICDData(string userName)
    {
        var query = new GetICDQuery(userName);
        var icdData = await _mediator.Send(query);
        return Ok(icdData);
    }
}