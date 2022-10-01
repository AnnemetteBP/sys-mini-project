using System;
using System.Collections.Generic;

namespace SharedModels
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int customerId { get; set; }
        public OrderStatusDto Status { get; set; }
        public IList<OrderLineDto> OrderLines { get; set; }

        public enum OrderStatusDto
        {
            tentative,
            cancelled,
            completed,
            shipped,
            paid
        }
    }

    public class OrderLineDto
    {
        public int id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
