using BasicGrpc.Contracts;
using Grpc.Net.Client;
using static BasicGrpc.Contracts.CustomerService;

using var channel = GrpcChannel.ForAddress("http://localhost:5000");
var client = new CustomerServiceClient(channel);
var reply = await client.ListCustomersAsync(new ListCustomersRequest());

foreach (var c in reply.Customers)
{
    Console.WriteLine("{0} {1}", c.FirstName, c.LastName);
}

var customerService = new CustomerServiceBase();
