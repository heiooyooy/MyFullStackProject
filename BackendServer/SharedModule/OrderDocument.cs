namespace SharedModule;

// 需要一个为ES优化的文档模型
public class OrderDocument
{
    public int Id { get; set; }
    public string Customer { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Amount { get; set; }
    public IList<OrderItem> Items { get; set; } // 反范式设计
}