using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ravigno.StockTransfer.Api.Data;
using Ravigno.StockTransfer.Api.DTOs;
using Ravigno.StockTransfer.Api.Models;

namespace Ravigno.StockTransfer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransfersController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransfersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransfers([FromQuery] int? branchId, [FromQuery] string? status)
    {
        var query = _context.StockTransfers
            .Include(t => t.Lines)
            .ThenInclude(l => l.Item)
            .AsQueryable();

        if (branchId.HasValue)
        {
            query = query.Where(t => t.FromBranchId == branchId.Value || t.ToBranchId == branchId.Value);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(t => t.Status.ToLower() == status.ToLower());
        }

        var results = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        return Ok(results);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransfer([FromBody] CreateTransferDto dto)
    {
        if (dto.FromBranchId == dto.ToBranchId)
            return BadRequest(new { message = "From and To branches must be different." });

        if (dto.Items == null || !dto.Items.Any())
            return BadRequest(new { message = "At least one item is required." });

        foreach (var item in dto.Items)
        {
            if (item.Quantity <= 0)
                return BadRequest(new { message = "Quantity must be greater than zero." });
        }

        var transfer = new StockTransferRecord
        {
            FromBranchId = dto.FromBranchId,
            ToBranchId = dto.ToBranchId,
            Status = "Requested",
            CreatedAt = DateTime.UtcNow,
            Lines = dto.Items.Select(i => new StockTransferLine
            {
                ItemId = i.ItemId,
                Quantity = i.Quantity
            }).ToList()
        };

        _context.StockTransfers.Add(transfer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTransfers), new { id = transfer.Id }, transfer);
    }

    [HttpPost("{id}/dispatch")]
    public async Task<IActionResult> DispatchTransfer(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var transfer = await _context.StockTransfers
                .Include(t => t.Lines)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transfer == null)
                return NotFound(new { message = "Transfer not found." });

            if (transfer.Status != "Requested")
                return BadRequest(new { message = $"Cannot dispatch transfer in status: {transfer.Status}" });

            foreach (var line in transfer.Lines)
            {
                var stock = await _context.BranchStocks
                    .FirstOrDefaultAsync(bs => bs.BranchId == transfer.FromBranchId && bs.ItemId == line.ItemId);

                if (stock == null || stock.Quantity < line.Quantity)
                {
                    return BadRequest(new { message = $"Insufficient stock for item ID {line.ItemId}. Stock cannot go below zero." });
                }

                stock.Quantity -= line.Quantity;
            }

            transfer.Status = "Dispatched";
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { message = "Transfer dispatched successfully.", transfer });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { message = "Error during dispatch.", error = ex.Message });
        }
    }

    [HttpPost("{id}/receive")]
    public async Task<IActionResult> ReceiveTransfer(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var transfer = await _context.StockTransfers
                .Include(t => t.Lines)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transfer == null)
                return NotFound(new { message = "Transfer not found." });

            if (transfer.Status != "Dispatched")
                return BadRequest(new { message = $"Cannot receive transfer in status: {transfer.Status}" });

            foreach (var line in transfer.Lines)
            {
                var receiverStock = await _context.BranchStocks
                    .FirstOrDefaultAsync(bs => bs.BranchId == transfer.ToBranchId && bs.ItemId == line.ItemId);

                if (receiverStock == null)
                {
                    _context.BranchStocks.Add(new BranchStock
                    {
                        BranchId = transfer.ToBranchId,
                        ItemId = line.ItemId,
                        Quantity = line.Quantity
                    });
                }
                else
                {
                    receiverStock.Quantity += line.Quantity;
                }
            }

            transfer.Status = "Received";
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { message = "Transfer received successfully.", transfer });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { message = "Error during receive.", error = ex.Message });
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelTransfer(int id)
    {
        var transfer = await _context.StockTransfers.FirstOrDefaultAsync(t => t.Id == id);

        if (transfer == null)
            return NotFound(new { message = "Transfer not found." });

        if (transfer.Status != "Requested")
            return BadRequest(new { message = $"Cannot cancel transfer in status: {transfer.Status}" });

        transfer.Status = "Cancelled";
        await _context.SaveChangesAsync();

        return Ok(new { message = "Transfer cancelled successfully.", transfer });
    }
}
