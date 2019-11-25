using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcChat.Shared;
using ProtoBuf.Grpc.Client;

namespace GrpcChat.Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            using var http = GrpcChannel.ForAddress("https://localhost:5001");
            var chatService = http.CreateGrpcService<IChat>();
            try
            {
                await foreach (var chat in chatService.JoinAndChat(SendChat()))
                {
                    Console.WriteLine($"from {chat.FromUserId},msg:{chat.Content}");
                }
            }
            catch (Exception ex)
            {
            }
        }

        private static async IAsyncEnumerable<Message> SendChat()
        {
            Console.WriteLine("login with id:");
            var userid = Console.ReadLine();
            if (!string.IsNullOrEmpty(userid))
            {
                yield return new Message() { FromUserId = userid, Content = "login" };
            }

            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                Console.WriteLine("send msg:");
                var msg = Console.ReadLine();

                if (msg == "exist")
                {
                    yield break;
                }

                var split = msg.Split(':');

                if (split.Length > 1)
                {
                    yield return new Message() { FromUserId = userid, ToUserId = split[0], Content = split[1] };
                }
                else
                {
                    yield return new Message() { FromUserId = userid, Content = split[1] };
                }
            }
        }
    }
}