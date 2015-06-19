using System;
using Akka.Actor;
using AkkaChat.Messages;

namespace ChatClient
{
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
}