using System;
using System.IO;
using Akka.Actor;
using Akka.TestKit.NUnit;
using AkkaChat.Messages;
using NUnit.Framework;
using Rhino.Mocks;

namespace ChatClient.Tests
{
    [TestFixture]
    public class ChatterActorTests : TestKit
    {
        [Test]
        public void when_told_to_receive_input_should_use_first_matching_input_handler()
        {
            var commandProcessor = MockRepository.GenerateMock<IProcessCommands>();

            commandProcessor.Stub(x => x.Process("\\Test")).Return(true);

            var chatter = ActorOfAsTestActorRef<ChatterActor>(Props.Create(() => new ChatterActor("", "", new[] { commandProcessor }, TestActor)));

            chatter.Tell(new ReceiveInput("\\Test"));

            commandProcessor.AssertWasCalled(x => x.Process("\\Test"));

        }

        [Test]
        public void when_told_to_receive_input_will_tell_console_reader_to_get_next_input()
        {
            var commandProcessor = MockRepository.GenerateMock<IProcessCommands>();

            commandProcessor.Stub(x => x.Process(Arg<string>.Is.Anything)).Return(true);

            var chatter = ActorOfAsTestActorRef<ChatterActor>(Props.Create(() => new ChatterActor("", "", new[] { commandProcessor }, TestActor)));

            chatter.Tell(new ReceiveInput("\\Test"));

            ExpectMsg<GetNextInput>();
        }

        [Test]
        public async void when_told_to_join_foo_room_as_bar_will_send_join_message()
        {
            var commandProcessor = MockRepository.GenerateMock<IProcessCommands>();

            commandProcessor.Stub(x => x.Process("\\Test")).Return(true);

            var consoleReader = Sys.ActorOf(Props.Create(() => new ConsoleReader(TextReader.Null, TextWriter.Null)), "consoleReader");
            var chatter = ActorOfAsTestActorRef<ChatterActor>(
                Props.Create(() =>new ChatterActor("[TEST]", "", new[] { commandProcessor }, consoleReader)), 
                "testChatterActor");

            var roomSupervisorSelection = Sys.ActorSelection(chatter.Path + "/roomSupervisorProxy");

            var s = await roomSupervisorSelection.Ask<ActorIdentity>(new Identify("foo"));
            Assert.NotNull(s.Subject);


            var actorToWatch = await roomSupervisorSelection.ResolveOne(TimeSpan.FromMilliseconds(500));
            Watch(actorToWatch);


            chatter.Tell(new ReceiveInput("\\join foo bar"));
            ExpectMsg<JoinRoom>(x => x.RoomName == "foo"); 

                
            
            //ExpectMsg(new JoinRoom {ChatterName = "bar", RoomName = "foo"});
        }
    }

    class foo
    {
        
    }
}
