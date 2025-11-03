using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleRestApi.Data;

namespace SimpleRestApi.Controllers
{

  [ApiController]
  [Route("api/[controller]")]
  public class ItemsController : ControllerBase
  {

    private readonly SimpleRestApiContext _context;

    public ItemsController (SimpleRestApiContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      var items = await _context.Items.ToListAsync();
      return Ok(items);
    }
  }
}