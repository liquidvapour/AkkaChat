using System.IO;
using Akka.Actor;
using AkkaChat.Messages;

namespace ChatClient
{
    public class ConsoleReader : TypedActor, IHandle<GetNextInput>
    {
        private readonly TextReader _inputReader;
        private readonly TextWriter _outputWriter;

        public ConsoleReader(TextReader inputReader, TextWriter outputWriter)
        {
            _inputReader = inputReader;
            _outputWriter = outputWriter;
        }

        public void Handle(GetNextInput getNextInput)
        {
            _outputWriter.Write("> ");
            var input = _inputReader.ReadLine();
            Sender.Tell(new ReceiveInput(input));
       }
    }
}