using Diagnostics.Tracing;

namespace _404.Aggregators
{
    public abstract class EventAggregator
    {
        public abstract void TraceEventAvailable(TraceEvent eventData);
    }
}