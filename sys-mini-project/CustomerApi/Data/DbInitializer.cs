using CustomerApi.Models;

namespace CustomerApi.Data
{
    public class DbInitializer : IDbInitializer
    {
        public void Initialize(CustomerApiContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any Products
            if (context.Customers.Any())
            {
                return;   // DB has been seeded
            }

            List<Customer> customers = new List<Customer>
            {
                new Customer { customerId = 1, name = "Jens", email = "Jens@email.dk", phone = "75884402", billingAddress = "gade 1", shippingAddress  = "gade 1", creditStanding = true},
                new Customer { customerId = 2, name = "Hans", email = "hans@email.dk", phone = "21763499", billingAddress = "vej 43", shippingAddress  = "vej 43", creditStanding = false},
                new Customer { customerId = 3, name = "Kurt", email = "kurt@email.dk", phone = "12324334", billingAddress = "gyden 1", shippingAddress  = "gyden 1", creditStanding = true},
            };

            context.Customers.AddRange(customers);
            context.SaveChanges();
        }
    }
}
