using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleRestApi.Data;

namespace SimpleRestApi.Controllers
{

  [ApiController]
  [Route("api/[controller]")]
  public class InvoicesController : ControllerBase
  {

    private readonly SimpleRestApiContext _context;

    public InvoicesController(SimpleRestApiContext context)
    {
      _context = context;
    }

    private static string GenerateInvoiceNumber(int invoiceId, DateTime invoiceDate)
    {
      var persianCalendar = new PersianCalendar();
      int year = persianCalendar.GetYear(invoiceDate);
      int month = persianCalendar.GetMonth(invoiceDate);
      int day = persianCalendar.GetDayOfMonth(invoiceDate);
      var dateString = $"{year:0000}{month:00}{day:00}{invoiceId:D4}";

      return dateString;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
      var invoices = await _context.Invoices.ToListAsync();
      return Ok(invoices);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Invoice newInvoice)
    {
      var invoiceDate = DateTime.Now;
      int statusId = 1;
      try
      {
        newInvoice.StatusId = statusId;
        newInvoice.InvoiceDate = DateOnly.FromDateTime(invoiceDate);

        foreach (var item in newInvoice.InvoiceItems)
        {
          var dbItem = await _context.Items.FindAsync(item.ItemId);
          if (dbItem == null)
            return BadRequest($"Item with ID {item.ItemId} not found.");

          item.Fee = dbItem.Price;
        }

        _context.Invoices.Add(newInvoice);
        await _context.SaveChangesAsync();

        string invoiceNumber = GenerateInvoiceNumber(newInvoice.Id, invoiceDate);
        newInvoice.InvoiceNumber = invoiceNumber;

        await _context.SaveChangesAsync();
        await _context.Entry(newInvoice).ReloadAsync();
        return StatusCode(201, newInvoice);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return StatusCode(500, new { message = ex.Message, trace = ex.StackTrace });
      }

    }
  }
}
