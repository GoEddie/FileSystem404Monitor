using System;
using Diagnostics.Tracing;
using _404.ProcessStore;

namespace _404.Aggregators
{
    internal class ProcessAggregator : EventAggregator
    {
        private readonly Action<ProcessStartEvent> _completed;
        private const string PayloadImageName = "ImageName";

        public ProcessAggregator(Action<ProcessStartEvent> completed)
        {
            _completed = completed;
        }

        public override void TraceEventAvailable(TraceEvent eventData)
        {
            if (eventData.Opcode == TraceEventOpcode.Start)
            {
                
                var processName = eventData.PayloadByName(PayloadImageName) as string;
             
                _completed(new ProcessStartEvent
                {
                    Name = processName,
                    Pid = eventData.ProcessID,
                    StartTime = eventData.TimeStamp
                });

                return;
            }
        }
    }
}