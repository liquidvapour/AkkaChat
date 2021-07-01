using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Akka.Actor;
using AkkaChat.Messages;

namespace ChatClient
{
    public class ChatterActor : ReceiveActor, IWithUnboundedStash
    {
        private readonly IEnumerable<IProcessCommands> _commandProcessors;
        private readonly IActorRef _roomSupervisor;
        private readonly IActorRef _consoleReader;
        private IActorRef _room;
        private readonly Regex _rex = new Regex(@"^\\join (?<room>[a-zA-Z0-9]*) (?<user>[a-zA-Z0-9]*)", RegexOptions.Compiled);

        public ChatterActor(string serverIp, string serverPort, IEnumerable<IProcessCommands> commandProcessors, IActorRef consoleReader)
        {
            _commandProcessors = commandProcessors;
            _consoleReader = consoleReader;
            _roomSupervisor = Context.ActorOf(Props.Create(() => new RoomSupervisorProxyActor(serverIp, serverPort)), "roomSupervisorProxy");
            Receive<GetNextInput>(m => Handle(m));
            Receive<Update>(m => Handle(m));
            Receive<ReceiveInput>(m => Handle(m));
            Receive<Welcome>(m => Handle(m));
        }

        private void Handle(Welcome message)
        {
            _room = Sender;
            Console.WriteLine("You have been welcomed to '{0}'.", message.Name);
            Stash.UnstashAll();
        }

        private void Handle(ReceiveInput message)
        {
            var input = message.Input;

            if (input == null)
            {
                Self.Tell(new GetNextInput());
                return;
            }
            

            if (!input.StartsWith("\\"))
            {
                SendSay(input);
            }
            else if (input.StartsWith("\\join"))
            {
                SendJoin(input);
            }
            else if (input.StartsWith("\\whoisin"))
            {
                SendWhoIsIn();
            }
            else if (input.Equals("\\quit"))
            {
                Context.System.Shutdown();
            }

            Self.Tell(new GetNextInput());

        }

        private void SendWhoIsIn()
        {
            _room.Tell(new WhoIsInRoom());
        }

        private void SendJoin(string input)
        {
            var matches = _rex.Match(input);

            var roomName = matches.Groups["room"].Value;
            var chatterName = matches.Groups["user"].Value;

            _roomSupervisor.Tell(new JoinRoom
            {
                RoomName = roomName, 
                ChatterName = chatterName
            });
        }

        private void SendSay(string input)
        {
            if (input == null) return;
            

            if (_room != null)
            {
                _room.Tell(new Say {Message = input});
            }
            else
            {
                Stash.Stash();
            }
        }

        private void Handle(Update message)
        {
            Console.WriteLine("<{0}> {2}: {1}", message.ChatLogEntry.On, message.ChatLogEntry.Message, message.ChatLogEntry.Who);
        }

        private void Handle(GetNextInput message)
        {
            _consoleReader.Tell(message);
        }

        public IStash Stash { get; set; }
    }

    public interface IProcessCommands
    {
        bool Process(string command);
    }
}