using Akka.Actor;
using Akka.TestKit.NUnit;
using AkkaChat.Messages;
using NUnit.Framework;

namespace AkkaChat.Actors.Tests
{
    [TestFixture]
    public class RoomSupervisorActorTests : TestKit
    {
        [Test]
        public void when_joining_room_will_receive_welcome_message()
        {
            var roomSupervisor = ActorOfAsTestActorRef(() => new RoomSupervisorActor());
       
            roomSupervisor.Tell(new JoinRoom {ChatterName = "Tester", RoomName = "Blue"});

            ExpectMsg<Welcome>(x => Assert.That(x.Name, Is.EqualTo("Blue")));
        }

        [Test]
        public void when_told_to_join_room_will_create_child_room()
        {
            var roomSupervisor = ActorOf(Props.Create(() => new RoomSupervisorActor()));

            //Sys.
        }
    }
}
