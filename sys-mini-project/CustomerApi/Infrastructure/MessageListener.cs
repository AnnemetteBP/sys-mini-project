using System.Threading;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using CustomerApi.Data;
using CustomerApi.Models;
using SharedModels;

namespace CustomerApi.Infrastructure
{
    public class MessageListener
    {
        IServiceProvider provider;
        string connectionString;
        IBus bus;

        // The service provider is passed as a parameter, because the class needs
        // access to the product repository. With the service provider, we can create
        // a service scope that can provide an instance of the product repository.
        public MessageListener(IServiceProvider provider, string connectionString)
        {
            this.provider = provider;
            this.connectionString = connectionString;
        }

        public void Start()
        {
            using (bus = RabbitHutch.CreateBus(connectionString))
            {
                bus.PubSub.Subscribe<OrderCreatedMessage>("customerApiAmCreated",
                    HandleOrderCreated);

                // Block the thread so that it will not exit and stop subscribing.
                lock (this)
                {
                    Monitor.Wait(this);
                }
            }

        }

        private void HandleOrderCreated(OrderCreatedMessage message)
        {
            // A service scope is created to get an instance of the product repository.
            // When the service scope is disposed, the product repository instance will
            // also be disposed.
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var customerRepo = services.GetService<IRepository<Customer>>();
                try
                {
                    var customer = customerRepo.Get((int)message.CustomerId);
                    if (customer.creditStanding)
                    {
                        var replyMessage = new OrderAllowedMessage
                        {
                            CustomerId = message.CustomerId,
                            OrderId = message.OrderId,
                            OrderLines = message.OrderLines
                        };

                        bus.PubSub.Publish(replyMessage);
                    }
                    else
                    {
                        // Publish an OrderRejectedMessage
                        var replyMessage = new OrderRejectedMessage
                        {
                            CustomerId = message.CustomerId,
                            OrderId = message.OrderId,
                            OrderLines = message.OrderLines
                        };

                        bus.PubSub.Publish(replyMessage);
                    }
                }
                catch(ArgumentNullException nullException)
                {
                    // Publish an OrderRejectedMessage
                    var replyMessage = new OrderRejectedMessage
                    {
                        CustomerId = message.CustomerId,
                        OrderId = message.OrderId,
                        OrderLines = message.OrderLines
                    };

                    bus.PubSub.Publish(replyMessage);
                }
            }
        }
    }
}
