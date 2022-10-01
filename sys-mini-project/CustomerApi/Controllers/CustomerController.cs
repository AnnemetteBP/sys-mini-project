using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.AspNetCore.Mvc;
using SharedModels;

namespace CustomerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IRepository<Customer> repository;

        public CustomersController(IRepository<Customer> repos)
        {
            repository = repos;
        }

        // GET customers
        [HttpGet]
        public IEnumerable<CustomerDto> Get()
        {
            var customers = new List<CustomerDto>();
            var converter = new CustomerConverter();
            foreach (var customer in repository.GetAll())
            {
                customers.Add(converter.Convert(customer));
            }
            return customers;
        }

        // GET customers/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public IActionResult Get(int id)
        {
            CustomerDto customer = new CustomerConverter().Convert(repository.Get(id));
            if (customer == null)
            {
                return NotFound();
            }
            return new ObjectResult(customer);
        }

        // POST customers
        [HttpPost]
        public IActionResult Post([FromBody] CustomerDto customer)
        {
            if (customer == null)
            {
                return BadRequest();
            }
            if (customer.customerId != null)
            {
                customer.customerId = null;
            }
            var newCustomer = repository.Add(new CustomerConverter().Convert(customer));

            return CreatedAtRoute("GetCustomer", new { id = newCustomer.customerId }, newCustomer);
        }

        // PUT customers/5
        [HttpPut("{id}")]
        public IActionResult Put([FromBody] CustomerDto customer)
        {
            if (customer == null || customer.customerId == null)
            {
                return BadRequest();
            }

            var modifiedCustomer = repository.Get((int)customer.customerId);

            if (modifiedCustomer == null)
            {
                return NotFound();
            }

            modifiedCustomer.name = customer.name;
            modifiedCustomer.email = customer.email;
            modifiedCustomer.phone = customer.phone;
            modifiedCustomer.billingAddress = customer.billingAddress;
            modifiedCustomer.shippingAddress = customer.shippingAddress;

            repository.Edit(modifiedCustomer);

            return new ObjectResult(modifiedCustomer);
        }

        // DELETE customers/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (repository.Get(id) == null)
            {
                return NotFound();
            }

            repository.Remove(id);
            return new ObjectResult(id);
        }
    }
}
