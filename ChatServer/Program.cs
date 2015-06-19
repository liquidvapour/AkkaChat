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
                    stdout-loglevel=DEBUG
                    loglevel = DEBUG
                    log-config-on-start = on        
                    actor {
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    }

                    remote {
                        helios.tcp {
                            port = [PORT]
                            hostname = localhost
                        }
                    }
                    debug {  
                          receive = on 
                          autoreceive = on
                          lifecycle = on
                          event-stream = on
                          unhandled = on
                    }
                }".Replace("[PORT]", port));

            using (var system = ActorSystem.Create("ChatServer", config))
            {
                RoomSupervisor = system.ActorOf(Props.Create(() => new AkkaChat.Actors.RoomSupervisorActor()), "roomSupervisor");
                system.AwaitTermination();
            }
        }

        private static IActorRef RoomSupervisor;
    }
}
