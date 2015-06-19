using Akka.Actor;
using Akka.Configuration;
using Mono.Options;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = "8080";

            var p = new OptionSet
            {
                {"p|port=", v => port = v}
            };

            p.Parse(args);

            var config = ConfigurationFactory.ParseString(
                @"akka {
                    actor {
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    }

                    remote {
                        helios.tcp {
                            port = [PORT]
                            hostname = localhost
                        }
                    }
                }".Replace("[PORT]", port));

            using (var system = ActorSystem.Create("ChatServer", config))
            {
                Room = system.ActorOf(Props.Create(() => new AkkaChat.Actors.RoomActor()), "room");
                system.AwaitTermination();
            }
        }

        private static IActorRef Room;
    }
}
