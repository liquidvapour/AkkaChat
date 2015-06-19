using System;
using System.Text.RegularExpressions;
using Akka.Actor;
using AkkaChat.Messages;

namespace ChatClient
{
    class ChatterActor : ReceiveActor, IWithUnboundedStash
    {
        private readonly IActorRef _roomSupervisor;
        private readonly IActorRef _consoleReader;
        private IActorRef _room;
        private readonly Regex _rex = new Regex(@"^\\join (?<room>[a-zA-Z0-9]*) (?<user>[a-zA-Z0-9]*)", RegexOptions.Compiled);

        public ChatterActor(string serverIp, string serverPort)
        {
            _consoleReader = Context.ActorOf(Props.Create(() => new ConsoleReader()));

            _roomSupervisor = Context.ActorOf(Props.Create(() => new RoomSupervisorProxyActor(serverIp, serverPort)));
            Receive<ProcessInput>(m => Handle(m));
            Receive<Update>(m => Handle(m));
            Receive<InputReceived>(m => Handle(m));
            Receive<Welcome>(m => Handle(m));
        }

        private void Handle(Welcome message)
        {
            _room = Sender;
            Console.WriteLine("You have been welcomed to '{0}'.", message.Name);
            Stash.UnstashAll();
        }

        private void Handle(InputReceived message)
        {
            var input = message.Input;
            if (!input.StartsWith("\\"))
            {
                Say(input);
            }
            else if (input.StartsWith("\\join"))
            {
                HandleJoin(input);
            }
            else if (input.StartsWith("\\whoisin"))
            {
                WhoIsIn();
            }
            else if (input.Equals("\\quit"))
            {
                Context.System.Shutdown();
            }

            Self.Tell(new ProcessInput());

        }

        private void WhoIsIn()
        {
            _room.Tell(new WhoIsInRoom());
        }

        private void HandleJoin(string input)
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

        private void Say(string input)
        {
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

        private void Handle(ProcessInput message)
        {
            _consoleReader.Tell(message);
        }

        public IStash Stash { get; set; }
    }
}