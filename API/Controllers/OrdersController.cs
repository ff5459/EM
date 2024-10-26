using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public OrdersController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file.ContentType != "text/csv")
            return BadRequest("Not a csv file");

        try
        {
            var orders = await CsvService.ParseAsync<Order>(file.OpenReadStream(), ',');

            await _dbContext.Orders.AddRangeAsync(orders);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok();
    }

    [HttpGet("download")]
    public async Task<IActionResult> Download([FromQuery] string region)
    {
        var firstOrder = await _dbContext.Orders
            .OrderBy(o => o.Date)
            .FirstOrDefaultAsync(o => o.Region == region);
        
        if (firstOrder == null)
            return NoContent();

        var orders = await _dbContext.Orders
            .Where(o => o.Region == region && o.Date >= firstOrder.Date && o.Date < firstOrder.Date + TimeSpan.FromMinutes(30))
            .OrderBy(o => o.Date)
            .AsNoTracking()
            .ToArrayAsync();

        var stream = new MemoryStream();
        await CsvService.WriteToStreamAsync(stream, orders);
        stream.Position = 0;

        return File(stream, "text/csv");
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string region)
    {
        var firstOrder = await _dbContext.Orders
            .OrderBy(o => o.Date)
            .FirstOrDefaultAsync(o => o.Region == region);
        
        if (firstOrder == null)
            return NoContent();

        var orders = await _dbContext.Orders
            .Where(o => o.Region == region && o.Date >= firstOrder.Date && o.Date < firstOrder.Date + TimeSpan.FromMinutes(30))
            .OrderBy(o => o.Date)
            .AsNoTracking()
            .ToArrayAsync();

        return Ok(orders);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        _dbContext.Orders.RemoveRange(_dbContext.Orders);
        
        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}