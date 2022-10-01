using System.Collections.Generic;

namespace SharedModels
{
    public class OrderCreatedMessage
    {
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public IList<OrderLineDto> OrderLines { get; set; }
    }
}
