using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrpcChat.Shared;

namespace GrpcChat.Server
{
    public class Chat : IChat
    {
        private static readonly IDictionary<string, Chat> Room = new Dictionary<string, Chat>();

        public string UserId { get; private set; }
        public Queue<Message> Message { get; } = new Queue<Message>();

        public async IAsyncEnumerable<Message> JoinAndChat(IAsyncEnumerable<Message> stream)
        {
            var task = Task.Run(async () =>
            {
                await foreach (var req in stream)
                {
                    if (string.IsNullOrEmpty(UserId))
                    {
                        UserId = req.FromUserId;
                        SysEcho("sys", $"user {req.FromUserId} is online.");

                        Room.TryAdd(req.FromUserId, this);
                        Message.Enqueue(new Message() { FromUserId = "sys", Content = "login_successful" });
                        continue;
                    }

                    if (string.IsNullOrEmpty(req.ToUserId))
                    {
                        SysEcho(req.FromUserId, $"user {req.FromUserId} is online.");
                    }

                    if (Room.TryGetValue(req.ToUserId, out var chat))
                    {
                        chat.Message.Enqueue(new Message() { Content = req.Content, FromUserId = req.FromUserId });
                    }
                }

                Room.Remove(UserId);
                SysEcho("sys", $"user {this.UserId} is offline.");
            });

            while (true)
            {
                while (Message.Any())
                {
                    var msg = Message.Dequeue();
                    yield return new Message()
                    {
                        FromUserId = msg.FromUserId,
                        Content = msg.Content,
                        ToUserId = this.UserId
                    };
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private void SysEcho(string from, string content)
        {
            foreach (var chat in Room.Values)
                chat.Message.Enqueue(new Message() { FromUserId = from, ToUserId = chat.UserId, Content = content });
        }
    }
}