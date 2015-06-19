using System;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaChat.Messages;

namespace ChatClient
{
    class ChatterActor : ReceiveActor
    {
        private readonly ActorSelection _room;
        private IActorRef _consoleReader;

        public ChatterActor(ActorSelection room)
        {
            _consoleReader = Context.ActorOf(Props.Create(() => new ConsoleReader()));

            _room = room;
            Receive<ProcessInput>(m => Handle(m));
            Receive<Update>(m => Handle(m));
            Receive<InputReceived>(m => Handle(m));
        }

        private void Handle(InputReceived message)
        {
            var input = message.Input;
            if (!input.StartsWith("\\"))
            {
                _room.Tell(new Say {Message = input});
            }
            else if (input.StartsWith("\\join"))
            {
                _room.Tell(new Join {Name = input.Substring(6)});
            }
            else if (input.Equals("\\quit"))
            {
                Context.System.Shutdown();
            }

            Self.Tell(new ProcessInput());

        }

        private void Handle(Update message)
        {
            Console.WriteLine("{0}: {1}", message.ChatLogEntry.On, message.ChatLogEntry.Message);
        }

        private void Handle(ProcessInput message)
        {
            _consoleReader.Tell(message);
        }


    }

    class ConsoleReader : ReceiveActor
    {
        public ConsoleReader()
        {
            Receive<ProcessInput>(m => Handle(m));
        }

        private void Handle(ProcessInput processInput)
        {
            var input = Console.ReadLine();
            Sender.Tell(new InputReceived(input));
        }
    }

    internal class InputReceived
    {
        public InputReceived(string input)
        {
            Input = input;
        }

        public string Input { get; private set; }
    }
}