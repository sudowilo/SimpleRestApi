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
      var invoices = await _context.Invoices
        .Include(i => i.InvoiceItems)
        .ThenInclude(ii => ii.Item)
        .Include(i => i.Status)
        .Select(i => new
        {
          i.Id,
          i.InvoiceNumber,
          i.CustomerName,
          i.CustomerPhone,
          i.InvoiceDate,
          i.TotalAmount,
          i.Note,
          Status = i.Status != null ? i.Status.Description : null,
          InvoiceItems = i.InvoiceItems.Select(ii => new
          {
            ii.Id,
            ii.ItemId,
            ii.Quantity,
            ii.Fee,
            ii.TotalPrice,
            ItemName = ii.Item!.Name,
            ItemPrice = ii.Item!.Price
          })
        })
        .ToListAsync();

      return Ok(invoices);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
      var invoice = await _context.Invoices
          .Include(i => i.InvoiceItems)
              .ThenInclude(ii => ii.Item)
          .Include(i => i.Status)
          .Where(i => i.Id == id)
          .Select(i => new
          {
            i.Id,
            i.InvoiceNumber,
            i.CustomerName,
            i.CustomerPhone,
            i.InvoiceDate,
            i.TotalAmount,
            i.Note,
            Status = i.Status != null ? i.Status.Description : null,
            InvoiceItems = i.InvoiceItems.Select(ii => new
            {
              ii.Id,
              ii.ItemId,
              ii.Quantity,
              ii.Fee,
              ii.TotalPrice,
              ItemName = ii.Item!.Name,
              ItemPrice = ii.Item!.Price
            })
          })
          .FirstOrDefaultAsync();

      if (invoice == null)
        return NotFound(new { message = "Invoice not found." });

      return Ok(invoice);
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

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] Invoice updatedInvoice)
    {
      var existingInvoice = await _context.Invoices
          .Include(i => i.InvoiceItems)
          .FirstOrDefaultAsync(i => i.Id == id);

      if (existingInvoice == null)
        return NotFound(new { message = "Invoice not found." });


      existingInvoice.CustomerName = updatedInvoice.CustomerName;
      existingInvoice.CustomerPhone = updatedInvoice.CustomerPhone;
      existingInvoice.Note = updatedInvoice.Note;
      existingInvoice.StatusId = updatedInvoice.StatusId;


      if (updatedInvoice.InvoiceItems != null && updatedInvoice.InvoiceItems.Count > 0)
      {
        _context.InvoiceItems.RemoveRange(existingInvoice.InvoiceItems);
        foreach (var item in updatedInvoice.InvoiceItems)
        {
          if (item == null)
            continue; 

          if (item.ItemId == 0)
            return BadRequest("Invoice item must have an ItemId.");

          item.InvoiceId = id;

          var dbItem = await _context.Items.FindAsync(item.ItemId);
          if (dbItem == null)
            return BadRequest($"Item with ID {item.ItemId} not found.");

          item.Fee = dbItem.Price;

          _context.InvoiceItems.Add(item);
        }
      }


      await _context.SaveChangesAsync();
      return Ok(new { message = "Invoice updated successfully." });
    }

  }
}
