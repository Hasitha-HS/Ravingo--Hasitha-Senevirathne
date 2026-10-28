using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ravigno.StockTransfer.Api.Data;

namespace Ravigno.StockTransfer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly AppDbContext _context;

    public BranchesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetBranches()
    {
        var branches = await _context.Branches.ToListAsync();
        return Ok(branches);
    }

    [HttpGet("{id}/stock")]
    public async Task<IActionResult> GetBranchStock(int id)
    {
        var stock = await _context.BranchStocks
            .Where(s => s.BranchId == id)
            .Include(s => s.Item)
            .Select(s => new
            {
                s.ItemId,
                ItemName = s.Item.Name,
                Sku = s.Item.Sku,
                s.Quantity
            })
            .ToListAsync();

        return Ok(stock);
    }
}