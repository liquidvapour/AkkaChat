using Akka.Actor;

namespace ChatClient
{
    class RoomSupervisorProxyActor : UntypedActor
    {
        private ActorSelection _roomSupervisor;
        private readonly string _serverIp;
        private readonly string _serverPort;

        public RoomSupervisorProxyActor(string serverIp, string serverPort)
        {
            _serverIp = serverIp;
            _serverPort = serverPort;
        }

        protected override void PreStart()
        {
            _roomSupervisor = Context.ActorSelection(string.Format("akka.tcp://ChatServer@{0}:{1}/user/roomSupervisor", _serverIp, _serverPort));
            base.PreStart();
        }

        protected override void OnReceive(object message)
        {
            if (_serverIp == "[TEST]") return;
            
            _roomSupervisor.Tell(message, Sender);
        }
    }
}