using Grpc.Core;
using MyCompany.CustomerService.Service.V1;
using MyCompany.CustomerService.Model.V1;

namespace CustomerService.Api.Services;

public class CustomerServiceImpl : MyCompany.CustomerService.Service.V1.CustomerService.CustomerServiceBase
{
    public async override Task<GetCustomerReply> GetCustomer(GetCustomerRequest request, ServerCallContext context)
    {
        var testCustomer = new Customer { FirstName = "Jane", LastName = "Doe" };

        Console.WriteLine(testCustomer.FullName);

        var result = new GetCustomerReply();
        result.Customers.Add(testCustomer);
        return result;
    }
}
