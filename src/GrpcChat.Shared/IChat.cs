using System.Collections.Generic;
using System.ServiceModel;

namespace GrpcChat.Shared
{
    [ServiceContract]
    public interface IChat
    {
        [OperationContract]
        IAsyncEnumerable<Message> JoinAndChat(IAsyncEnumerable<Message> message);
    }
}