namespace Ravigno.StockTransfer.Api.DTOs;

public class CreateTransferDto
{
    public int FromBranchId { get; set; }
    public int ToBranchId { get; set; }
    public List<TransferItemDto> Items { get; set; } = new();
}

public class TransferItemDto
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}
