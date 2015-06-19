using System;

namespace AkkaChat.Messages
{
    public class ChatLogEntry
    {
        public DateTime On { get; set; }
        public string Message { get; set; }
        public string Who { get; set; }
    }
}