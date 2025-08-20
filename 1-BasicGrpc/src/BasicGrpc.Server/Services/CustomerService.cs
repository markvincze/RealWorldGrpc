using BasicGrpc.Contracts;
using Grpc.Core;
using static BasicGrpc.Contracts.CustomerService;

namespace BasicGrpc.Server;

public class CustomerService : CustomerServiceBase
{
    private static readonly List<Customer> customers = new List<Customer>();

    public async override Task<CreateCustomerReply> CreateCustomer(CreateCustomerRequest request, ServerCallContext context)
    {
        customers.Add(new Customer { FirstName = request.FirstName, LastName = request.LastName });

        return new CreateCustomerReply();
    }

    public async override Task<ListCustomersReply> ListCustomers(ListCustomersRequest request, ServerCallContext context)
    {
        var reply = new ListCustomersReply();

        foreach (var c in customers)
        {
            reply.Customers.Add(c);
        }

        return reply;
    }
}