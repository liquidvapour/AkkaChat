using System;
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

            var showHelp = false;
            var p = new OptionSet
            {
                { "p|port=", "The port this client will listen on. Defaults to '8090'.", v => port = v},
                { "h|hostname=", "This client's host name. Defaults to 'localhost'.", v => machine = v},
                { "s|server=", "The address of the chat server. defaults to 'localhost'.", v => serverIp = v},
                { "r|serverPort=", "The port of the chat server. Defaults to '8080'.", v => serverPort = v},
                { "?|help", "Shows this help and exits.", v => showHelp = v != null}
            };

            p.Parse(args);

            if (showHelp)
            {
                ShowHelp(p);
                return;
            }
            
            Greet();

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

            hocon = hocon.Replace("[PORT]", port);
            hocon = hocon.Replace("[MACHINE]", machine);

            var config = ConfigurationFactory.ParseString(
                hocon);



            var chatSystem = ActorSystem.Create("ChatSystem", config);


            var roomSupervisor = chatSystem.ActorSelection(string.Format("akka.tcp://ChatServer@{0}:{1}/user/roomSupervisor", serverIp, serverPort));
            var chatter = chatSystem.ActorOf(Props.Create(() => new ChatterActor(roomSupervisor)), "chatter");
            
            chatter.Tell(new ProcessInput());

            chatSystem.AwaitTermination();
        }

        private static void Greet()
        {
            Console.WriteLine("---------------------------------");
            Console.WriteLine(" Welcome to Akka.Net Chat Client");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Use -? or --help on the command line for more start up options.");
            Console.WriteLine();
            Console.WriteLine("Try the following at the console:");
            Console.WriteLine("'\\join <room> <yourName>' to join a room. Then just type a line and press <Enter> to start chatting.");
            Console.WriteLine("'\\whoisin' Once you are in a room to see who is in the room with you.");
        }

        private static void ShowHelp(OptionSet optionSet)
        {
            Console.WriteLine("Akka.Net Commandline Chat Client -by Liquidvapour");
            Console.WriteLine("Options:");
            optionSet.WriteOptionDescriptions(Console.Out);
        }
    }
}
