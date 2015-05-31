using System;
using Diagnostics.Tracing;
using Diagnostics.Tracing.Parsers;

namespace _404.Etw
{
    public abstract class EventAggregator
    {
        public abstract void TraceEventAvailable(TraceEvent eventData);
    }
    
    public class EtwEventProvider
    {
        private readonly EventAggregator _aggregator;
        private readonly ulong _keywords;
        private readonly string _provider;
        private readonly string _sessionName;
        private TraceEventSession _session;

        public EtwEventProvider(string sessionName, string provider, ulong keywords, EventAggregator aggregator)
        {
            _sessionName = sessionName;
            _provider = provider;
            _keywords = keywords;
            _aggregator = aggregator;
        }

        public bool Start()
        {
            try
            {
                if (!(TraceEventSession.IsElevated() ?? false))
                {
                    Console.WriteLine(
                        "To turn on ETW events you need to be Administrator, please run from an Admin process.");
                }

                CreateSession();

                using (var source = new ETWTraceEventSource(_sessionName, TraceEventSourceType.Session))
                {
                    RegisterParser(source);

                    if (!EnableProvider())
                        return false;

                    source.Process();

                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        private bool EnableProvider()
        {
            var processProviderGuid = TraceEventSession.GetProviderByName(_provider);
            
            if (processProviderGuid == Guid.Empty)
            {
                Console.WriteLine("Error could not find {0} etw provider.", _provider);
                return false;
            }
            _session.EnableProvider(processProviderGuid, TraceEventLevel.Verbose, _keywords); /*0x10c0*/
            return true;
        }

        private RegisteredTraceEventParser RegisterParser(ETWTraceEventSource source)
        {
            var registeredParser = new RegisteredTraceEventParser(source);
            registeredParser.All += _aggregator.TraceEventAvailable;
            return registeredParser;
        }

        private void CreateSession()
        {
            _session = new TraceEventSession(_sessionName, null /*Real-time*/);
            _session.StopOnDispose = true;
        }

        public void Stop()
        {
            _session.Dispose();
        }

        ~EtwEventProvider()
        {
            _session.Dispose();
        }
    }
}
