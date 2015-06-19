using System;
using System.Linq;
using Akka.Actor;
using AkkaChat.Messages;

namespace AkkaChat.Actors
{
    public class RoomSupervisorActor : ReceiveActor
    {
        public RoomSupervisorActor()
        {
            Receive<JoinRoom>(m => Handle(m));
        }

        private void Handle(JoinRoom message)
        {
            var room = Context.GetChildren()
                .SingleOrDefault(x => x.Path.Name.Equals(message.RoomName, StringComparison.OrdinalIgnoreCase))
                       ?? Context.ActorOf(Props.Create(() => new RoomActor(message.RoomName)), message.RoomName);

            room.Tell(new Join { Name = message.ChatterName}, Sender);
        }
    }
}