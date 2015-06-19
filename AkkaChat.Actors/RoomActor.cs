using System;
using System.Collections.Generic;
using Akka.Actor;
using AkkaChat.Messages;

namespace AkkaChat.Actors
{
    public class RoomActor : ReceiveActor
    {
        private readonly IList<ChatLogEntry> _log = new List<ChatLogEntry>();
        private readonly IDictionary<string, IActorRef> _members = new Dictionary<string, IActorRef>();

        public RoomActor()
        {
            Receive<Say>(m => Handle(m));
            Receive<Join>(m => Handle(m));
        }

        private void Handle(Join message)
        {
            _members.Add(message.Name, Sender);
            Self.Tell(new Say{ Message = string.Format("{0} has joined", message.Name)});
        }

        private void Handle(Say say)
        {
            var chatLogEntry = new ChatLogEntry {Message = say.Message, On = DateTime.Now};
            _log.Add(chatLogEntry);
            Publish(chatLogEntry);
        }

        private void Publish(ChatLogEntry chatLogEntry)
        {
            foreach (var member in _members.Values)
            {
                member.Tell(new Update {ChatLogEntry = chatLogEntry});
            }
        }
    }
}