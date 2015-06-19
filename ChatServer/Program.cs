using Akka.Actor;
using Akka.Configuration;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(
                @"akka {
                    actor {
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    }

                    remote {
                        helios.tcp {
                            port = 8080
                            hostname = localhost
                        }
                    }
                }");

            using (var system = ActorSystem.Create("ChatServer", config))
            {
                Room = system.ActorOf(Props.Create(() => new AkkaChat.Actors.RoomActor()), "room");
                system.AwaitTermination();
            }
        }

        private static IActorRef Room;
    }
}
