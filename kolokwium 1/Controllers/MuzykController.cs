using kolokwium_1.DTO;
using kolokwium_1.Models;
using kolokwium_1.Services;
using Microsoft.AspNetCore.Mvc;

namespace kolokwium_1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MuzykController : ControllerBase
{
    private IMuzykService _service;

    public MuzykController(IMuzykService service)
    {
        _service = service;
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMuzyk(int id)
    {
        Muzyk? muzyk = await _service.GetMuzyk(id);

        if (muzyk == null)
        {
            return NotFound();
        }

        return Ok(muzyk);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMuzyk(MuzykDTO muzyk)
    {
        try
        {
            int id = await _service.CreateMuzyk(muzyk);
            return Created();
        }
        catch (Exception e)
        {
            return BadRequest();
        }
    }
}