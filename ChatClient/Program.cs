using Akka.Actor;
using Akka.Configuration;
using AkkaChat.Messages;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var hocon = @"akka {
                    actor {
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    }

                    remote {
                        helios.tcp {
                            port = [PORT]
                            hostname = [MACHINE]
                        }
                    }
                }";

            var port = "8090";
            var machine = "localhost";

            if (args.Length > 0)
            {
                machine = args[0];
            }

            if (args.Length > 1)
            {
                port = args[1];
            }

            hocon = hocon.Replace("[PORT]", port);
            hocon = hocon.Replace("[MACHINE]", machine);

            var config = ConfigurationFactory.ParseString(
                hocon);



            var chatSystem = ActorSystem.Create("ChatSystem", config);

            var room = chatSystem.ActorSelection("akka.tcp://ChatServer@localhost:8080/user/room");
            var chatter = chatSystem.ActorOf(Props.Create(() => new ChatterActor(room)), "chatter");
            
            chatter.Tell(new ProcessInput());

            chatSystem.AwaitTermination();
        }
    }
}
