using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akka.Actor;
using Akka.Event;
using AkkaChat.Messages;

namespace AkkaChat.Actors
{
    public class RoomActor : ReceiveActor
    {
        private readonly string _roomName;
        private readonly IList<ChatLogEntry> _chatLog = new List<ChatLogEntry>();
        private readonly IDictionary<string, IActorRef> _members = new Dictionary<string, IActorRef>();
        private readonly ILoggingAdapter _log = Context.GetLogger();

        public RoomActor(string roomName)
        {
            _roomName = roomName;
            Receive<Say>(m => Handle(m));
            Receive<Join>(m => Handle(m));
            Receive<WhoIsInRoom>(m => Handle(m));
        }

        private void Handle(WhoIsInRoom message)
        {

            var roomList = new StringBuilder();

            roomList.AppendLine("all current room memebers:");

            foreach (var key in _members.Keys)
            {
                roomList.AppendLine(key);
            }

            roomList.AppendLine(new string('-', 10));

            Context.Sender.Tell(new Update {ChatLogEntry = new ChatLogEntry {Message = roomList.ToString(), On = DateTime.Now, Who = _roomName}});
        }

        private void Handle(Join message)
        {
            _log.Info("'{0}' joined room.", message.Name);
            _members.Add(message.Name, Sender);
            Sender.Tell(new Welcome {Name = _roomName});
            Self.Tell(new Say{ Message = string.Format("{0} has joined", message.Name)});
        }

        private void Handle(Say say)
        {
            _log.Debug("'{0}' said '{1}' in {2}.", Sender.Path, say.Message, _roomName);
            var chatLogEntry = new ChatLogEntry {Who = GetNameBy(Sender), Message = say.Message, On = DateTime.Now};
            _chatLog.Add(chatLogEntry);
            Publish(chatLogEntry);
        }

        private string GetNameBy(IActorRef sender)
        {
            foreach (var item in _members.Where(item => item.Value.Equals(sender)))
            {
                return item.Key;
            }

            return sender.Equals(Self) ? "The Room" : string.Empty;
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