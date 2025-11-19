using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Newtonsoft.Json;

namespace RealWorldGrpc.Streaming.ChatServer.Services;

public record ChatMessage(DateTime Sent, string NickName, string Message)
{
}

public class ChatService(ILogger<ChatService> logger) : ChatServer.ChatService.ChatServiceBase
{
    private static readonly List<ChatMessage> chatMessages = [];
    private static readonly HashSet<Func<ChatMessage, Task>> messageListeners = [];

    public override async Task ExchangeMessages(IAsyncStreamReader<ExchangeMessagesRequest> requestStream, IServerStreamWriter<ExchangeMessagesReply> responseStream, ServerCallContext context)
    {
        if (chatMessages.Count > 0)
        {
            var reply = new ExchangeMessagesReply();
            reply.Messages.AddRange(chatMessages.Select(cm => new SentMessage { Sent = Timestamp.FromDateTime(cm.Sent), NickName = cm.NickName, Message = cm.Message }));
            await responseStream.WriteAsync(reply);
        }

        var listener = async (ChatMessage msg) =>
        {
            var reply = new ExchangeMessagesReply();
            reply.Messages.Add(
                new SentMessage { Sent = Timestamp.FromDateTime(msg.Sent), NickName = msg.NickName, Message = msg.Message });
            await responseStream.WriteAsync(reply);
        };

        try
        {
            messageListeners.Add(listener);

            await foreach (var req in requestStream.ReadAllAsync())
            {
                Console.WriteLine("Received message. From: {0}, Message: {1}", req.NickName, req.Message);

                var now = DateTime.UtcNow;
                var msg = new ChatMessage(now, req.NickName, req.Message);
                chatMessages.Add(msg);

                foreach (var l in messageListeners)
                {
                    l(msg);
                }
            }
        }
        finally
        {
            messageListeners.Remove(listener);
        }
    }

    public override async Task SearchProducts(SearchProductsRequest request, IServerStreamWriter<SearchProductsReply> responseStream, ServerCallContext context)
    {
        for (int i = 0; i < 5; i++)
        {
            var reply = new SearchProductsReply();
            reply.Products.Add($"Product {i}");

            await responseStream.WriteAsync(reply);

            await Task.Delay(500);
        }
    }

    public override async Task<UploadFragmentsReply> UploadFragments(IAsyncStreamReader<UploadFragmentsRequest> requestStream, ServerCallContext context)
    {
        await foreach (var req in requestStream.ReadAllAsync(context.CancellationToken))
        {
            logger.LogInformation("Message received: {request}", JsonConvert.SerializeObject(req));
        }

        return new UploadFragmentsReply
        {
            Status = "Success"
        };
    }
}
