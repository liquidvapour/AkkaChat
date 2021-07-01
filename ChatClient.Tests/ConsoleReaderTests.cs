using System.IO;
using Akka.Actor;
using Akka.TestKit.NUnit;
using AkkaChat.Messages;
using NUnit.Framework;

namespace ChatClient.Tests
{
    [TestFixture]
    public class ConsoleReaderTests : TestKit
    {
        [Test]
        public void when_told_to_process_input_should_read_from_input_stream_and_pass_that_onto_receiver()
        {
            TextReader textReader = new StringReader("foo");
            var textWriter = new StringWriter();
            var consoleReader = Sys.ActorOf(Props.Create(() => new ConsoleReader(textReader, textWriter)), "consoleReader");

            consoleReader.Tell(new GetNextInput());

            ExpectMsg<ReceiveInput>(x => x.Input == "foo", hint: "the message contents to match.");
        }

        [Test]
        public void when_told_to_process_input_write_prompt_to_output()
        {
            TextReader textReader = new StringReader("foo");
            var textWriter = new StringWriter();

            var consoleReader = ActorOfAsTestActorRef<ConsoleReader>(Props.Create(() => new ConsoleReader(textReader, textWriter)), "consoleReader01");

            consoleReader.Tell(new GetNextInput());

            
            var actual = textWriter.ToString();
            Assert.That(actual, Is.EqualTo("> "));
        }
    }
}