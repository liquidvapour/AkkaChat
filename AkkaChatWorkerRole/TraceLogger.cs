using System.Diagnostics;
using Akka.Actor;
using Akka.Event;
using Debug = Akka.Event.Debug;

namespace AkkaChatWorkerRole
{
    public class TraceLogger : ReceiveActor
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogLogger"/> class.
        /// </summary>
        public TraceLogger()
        {
            Receive<Error>(m => Handle(m));
            Receive<Warning>(m => Handle(m));
            Receive<Info>(m => Handle(m));
            Receive<Debug>(m => Handle(m));
            Receive<InitializeLogger>(m =>
            {
                Trace.TraceInformation("NLogLogger started");
                Sender.Tell(new LoggerInitialized());
            });
        }

        private void Handle(Debug message)
        {
            Trace.TraceInformation("{0} {1}: {2}", message.Timestamp, message.LogSource, message.Message);
        }

        private void Handle(Info message)
        {
            Trace.TraceInformation("{0} {1}: {2}", message.Timestamp, message.LogSource, message.Message);
        }

        private void Handle(Warning message)
        {
            Trace.TraceWarning("{0} {1}: {2}", message.Timestamp, message.LogSource, message.Message);
        }

        private void Handle(Error error)
        {
            Trace.TraceError("{0} {1}: {2} {3}", error.Timestamp, error.LogSource, error.Message, error.Cause);
        }
    }
}