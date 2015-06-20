using System.Diagnostics;
using System.Net;
using Akka.Actor;
using Akka.Configuration;
using AkkaChat.Actors;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AkkaChatWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private ActorSystem _actorSystem;
        private IActorRef _actor;

        public override void Run()
        {
            Trace.TraceInformation("AkkaSkipeWorker is running");

            _actorSystem.AwaitTermination();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            _actorSystem = CreateServer();

            _actor = _actorSystem.ActorOf(Props.Create(() => new RoomSupervisorActor()), "roomSupervisor"); ;

            bool result = base.OnStart();

            Trace.TraceInformation("AkkaSkipeWorker has been started");

            return result;
        }



        public override void OnStop()
        {
            Trace.TraceInformation("AkkaSkipeWorker is stopping");

            _actorSystem.Shutdown();

            base.OnStop();

            Trace.TraceInformation("AkkaSkipeWorker has stopped");
        }

        private ActorSystem CreateServer()
        {
            var config = ConfigurationFactory.ParseString(
                @"akka {
                    loggers = [""AkkaChatWorkerRole.TraceLogger, AkkaChatWorkerRole""]
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
                }".Replace("[PORT]", "10100"));

            return ActorSystem.Create("ChatServer", config);
        }
    }
}
