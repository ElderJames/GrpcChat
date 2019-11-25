using System;
using System.Runtime.Serialization;

namespace GrpcChat.Shared
{
    [DataContract]
    public class Message
    {
        [DataMember(Order = 1)]
        public string FromUserId { get; set; }

        [DataMember(Order = 2)]
        public string ToUserId { get; set; }

        [DataMember(Order = 3)]
        public string Content { get; set; }

        [DataMember(Order = 4)]
        public DateTime Time { get; set; }
    }
}