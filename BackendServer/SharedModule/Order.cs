namespace SharedModule;

public class Order
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
}