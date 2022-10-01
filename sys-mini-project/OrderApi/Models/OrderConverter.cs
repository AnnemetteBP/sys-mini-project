using SharedModels;

namespace OrderApi.Models
{
    public class OrderConverter : IConverter<Order, OrderDto>
    {
        public Order Convert(OrderDto sharedOrder)
        {
            return new Order
            {
                customerId = sharedOrder.customerId,
                Date = sharedOrder.Date,
                Id = sharedOrder.Id,
                OrderLines = Convert(sharedOrder.OrderLines),
                Status = (Order.OrderStatus)sharedOrder.Status
            };
        }

        public OrderDto Convert(Order hiddenOrder)
        {
            return new OrderDto
            {
                customerId = hiddenOrder.customerId,
                Date = hiddenOrder.Date,
                Id = hiddenOrder.Id,
                OrderLines = Convert(hiddenOrder.OrderLines),
                Status = (OrderDto.OrderStatusDto)hiddenOrder.Status
            };
        }

        public IList<OrderLine> Convert(IList<OrderLineDto> sharedOrderLines)
        {
            var orderLines = new List<OrderLine>();
            foreach (var line in sharedOrderLines)
            {
                orderLines.Add(new OrderLine() { 
                    id = line.id,
                    ProductId = line.ProductId,
                    Quantity = line.Quantity
                });
            }
            return orderLines;
        }

        public IList<OrderLineDto> Convert(IList<OrderLine> hiddenOrderLines)
        {
            var orderLines = new List<OrderLineDto>();
            foreach (var line in hiddenOrderLines)
            {
                orderLines.Add(new OrderLineDto()
                {
                    id = line.id,
                    ProductId = line.ProductId,
                    Quantity = line.Quantity
                });
            }
            return orderLines;
        }
    }
}
