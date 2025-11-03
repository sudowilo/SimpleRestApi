using Microsoft.AspNetCore.Mvc;

namespace SimpleRestApi.Controllers
{

  [ApiController]
  [Route("api/[controller]")]
  public class ItemsController : ControllerBase
  {
    [HttpGet]
    public IActionResult Get()
    {
      return Ok("here is item route");
    }
  }
}