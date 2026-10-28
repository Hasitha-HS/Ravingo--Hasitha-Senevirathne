namespace Ravigno.StockTransfer.Api.Models;

public class Branch
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public ICollection<BranchStock> Stocks { get; set; } = new List<BranchStock>();
}