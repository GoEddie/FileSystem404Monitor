using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diagnostics.Tracing;
using _404.Etw;
using _404.ProcessStore;

namespace _404.Aggregators
{
    class ProcessAggregator : EventAggregator
    {
        private readonly Action<ProcessStartEvent> _completed;

        public ProcessAggregator(Action<ProcessStartEvent> completed)
        {
            _completed = completed;

        }

        public override void TraceEventAvailable(TraceEvent eventData)
        {
            if (eventData.Opcode == TraceEventOpcode.Start)
            {
                var processName = eventData.PayloadByName("ImageName") as string;
                Console.WriteLine("Process Aggregator: New Proc: {0}", processName);
                _completed(new ProcessStartEvent()
                {
                    Name=processName, Pid = eventData.ProcessID, StartTime = eventData.TimeStamp
                });

                return;
            }

            return;
        }
    }
}
