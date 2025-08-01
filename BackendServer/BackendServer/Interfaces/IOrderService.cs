using SharedModule;

namespace BackendServer.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(Order order);
    Task<Order> GetOrderById(int id);
}