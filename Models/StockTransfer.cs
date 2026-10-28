namespace Ravigno.StockTransfer.Api.Models;

public class StockTransferRecord
{
    public int Id { get; set; }
    public int FromBranchId { get; set; }
    public int ToBranchId { get; set; }
    public string Status { get; set; } = "Requested"; 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<StockTransferLine> Lines { get; set; } = new List<StockTransferLine>();
}

public class StockTransferLine
{
    public int Id { get; set; }
    public int StockTransferRecordId { get; set; }
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public int Quantity { get; set; }
}