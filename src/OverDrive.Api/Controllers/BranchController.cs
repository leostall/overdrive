using Microsoft.AspNetCore.Mvc;
using OverDrive.Api.Dtos.Branches;
using OverDrive.Api.Exceptions;
using OverDrive.Api.Services.Interfaces;

namespace OverDrive.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchController : ControllerBase
{
    private readonly IBranchService _service;

    public BranchController(IBranchService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateBranchRequest request)
    {
        try
        {
            var objResult = _service.Create(request);
            return Ok(objResult);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Unexpected error." });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateBranchRequest request)
    {
        try
        {
            var objResult = _service.Update(id, request);
            return Ok(objResult);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Unexpected error." });
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        try
        {
            var res = _service.GetById(id);
            return Ok(res);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Unexpected error." });
        }
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            var res = _service.GetAll();
            return Ok(res);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Unexpected error." });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            _service.Delete(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Unexpected error." });
        }
    }

    [HttpGet("{id}/report")]
    public IActionResult GetReportById(int id)
    {
        try
        {
            var res = _service.GetReportById(id);
            return Ok(res);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Unexpected error." });
        }
    }

    [HttpGet("reports")]
    public IActionResult GetReports()
    {
        try
        {
            var res = _service.GetReports();
            return Ok(res);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Unexpected error." });
        }
    }
}
