using System;
using System.Threading;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Data;
using OrderApi.Models;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public class MessageListener
    {
        IServiceProvider provider;
        string connectionString;
        IBus bus;

        // The service provider is passed as a parameter, because the class needs
        // access to the product repository. With the service provider, we can create
        // a service scope that can provide an instance of the order repository.
        public MessageListener(IServiceProvider provider, string connectionString)
        {
            this.provider = provider;
            this.connectionString = connectionString;
        }

        public void Start()
        {
            using (bus = RabbitHutch.CreateBus(connectionString))
            {
                bus.PubSub.Subscribe<OrderAllowedMessage>("orderApiAmAllowed",
                    HandleOrderAllowed);

                bus.PubSub.Subscribe<OrderAcceptedMessage>("orderApiAmAccepted",
                    HandleOrderAccepted);

                bus.PubSub.Subscribe<OrderRejectedMessage>("orderApiAmRejected",
                    HandleOrderRejected);

                // Block the thread so that it will not exit and stop subscribing.
                lock (this)
                {
                    Monitor.Wait(this);
                }
            }

        }

        private void HandleOrderAllowed(OrderAllowedMessage newOrder)
        {
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var messagePublisher = services.GetService<IMessagePublisher>();
                // Publish OrderStatusChangedMessage. 
                messagePublisher.PublishOrderReservedMessage(newOrder.CustomerId, newOrder.OrderId, newOrder.OrderLines);
            }
        }

        private void HandleOrderAccepted(OrderAcceptedMessage message)
        {
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var orderRepos = services.GetService<IRepository<Order>>();
                // Mark order as completed
                var order = orderRepos.Get(message.OrderId);
                order.Status = Order.OrderStatus.completed;
                orderRepos.Edit(order);
            }
        }

        private void HandleOrderRejected(OrderRejectedMessage message)
        {
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var orderRepos = services.GetService<IRepository<OrderDto>>();

                // Delete tentative order.
                orderRepos.Remove(message.OrderId);
            }
        }
    }
}
