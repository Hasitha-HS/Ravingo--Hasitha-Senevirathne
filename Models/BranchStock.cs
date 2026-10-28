namespace Ravigno.StockTransfer.Api.Models;

public class BranchStock
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public int Quantity { get; set; }
}