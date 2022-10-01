using SharedModels;

namespace CustomerApi.Models
{
    public class CustomerConverter : IConverter<Customer, CustomerDto>
    {
        public Customer Convert(CustomerDto sharedCustomer)
        {
            return new Customer()
            {
                customerId = (int)sharedCustomer.customerId,
                name = sharedCustomer.name,
                email = sharedCustomer.email,
                phone = sharedCustomer.phone,
                billingAddress = sharedCustomer.billingAddress,
                shippingAddress = sharedCustomer.shippingAddress,
                creditStanding = sharedCustomer.creditStanding
            };
        }

        public CustomerDto Convert(Customer hiddenCustomer)
        {
            return new CustomerDto()
            {
                customerId = hiddenCustomer.customerId,
                name = hiddenCustomer.name,
                email = hiddenCustomer.email,
                phone = hiddenCustomer.phone,
                billingAddress = hiddenCustomer.billingAddress,
                shippingAddress = hiddenCustomer.shippingAddress,
                creditStanding = hiddenCustomer.creditStanding
            };
        }
    }
}
