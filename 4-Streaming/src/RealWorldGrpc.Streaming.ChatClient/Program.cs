using Grpc.Core;
using Grpc.Net.Client;
using RealWorldGrpc.Streaming.ChatServer;

async Task ProcessIncomingMessages(IAsyncStreamReader<ExchangeMessagesReply> stream)
{
    await foreach (var response in stream.ReadAllAsync())
    {
        foreach (var incomingMessage in response.Messages)
        {
            Console.WriteLine(
                "[{0:G}] {1}: {2}",
                incomingMessage.Sent.ToDateTime(),
                incomingMessage.NickName,
                incomingMessage.Message);
        }
    }
}

Console.Write("Enter your nickname: ");
var nickName = Console.ReadLine();

var channel = GrpcChannel.ForAddress("http://localhost:5000");
var client = new ChatService.ChatServiceClient(channel);

var call = client.ExchangeMessages();

Console.CancelKeyPress += (s, e) =>
{
    call.RequestStream.CompleteAsync().Wait();
};

Task.Run(() => ProcessIncomingMessages(call.ResponseStream));

while (true)
{
    var message = Console.ReadLine();
    await call.RequestStream.WriteAsync(new ExchangeMessagesRequest { NickName = nickName, Message = message });
}
