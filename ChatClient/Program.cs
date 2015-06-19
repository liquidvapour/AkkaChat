using Akka.Actor;
using Akka.Configuration;
using AkkaChat.Messages;
using Mono.Options;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {

            var port = "8090";
            var machine = "localhost";
            var serverIp = "localhost";
            var serverPort = "8080";

            var p = new OptionSet
            {
                { "p|port=", v => port = v},
                { "h|hostname=", v => machine = v},
                { "s|server=", v => serverIp = v},
                { "r|serverPort=", v => serverPort = v}
            };

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


            p.Parse(args);

            hocon = hocon.Replace("[PORT]", port);
            hocon = hocon.Replace("[MACHINE]", machine);

            var config = ConfigurationFactory.ParseString(
                hocon);



            var chatSystem = ActorSystem.Create("ChatSystem", config);

            
            var room = chatSystem.ActorSelection(string.Format("akka.tcp://ChatServer@{0}:{1}/user/room", serverIp, serverPort));
            var chatter = chatSystem.ActorOf(Props.Create(() => new ChatterActor(room)), "chatter");
            
            chatter.Tell(new ProcessInput());

            chatSystem.AwaitTermination();
        }
    }
}
