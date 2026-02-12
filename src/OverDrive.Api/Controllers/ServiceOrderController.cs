using Microsoft.AspNetCore.Mvc;
using OverDrive.Api.Dtos.ServiceOrders;
using OverDrive.Api.Exceptions;
using OverDrive.Api.Services.Interfaces;

namespace OverDrive.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceOrderController : ControllerBase
{
    private readonly IServiceOrderService _service;

    public ServiceOrderController(IServiceOrderService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateServiceOrderRequest request)
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
    public IActionResult Update(int id, [FromBody] UpdateServiceOrderRequest request)
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
}