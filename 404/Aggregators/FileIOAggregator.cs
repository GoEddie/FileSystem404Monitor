using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diagnostics.Tracing;
using _404.Etw;

namespace _404.Aggregators
{
    public class FileIOAggregator : EventAggregator
    {
        public override void TraceEventAvailable(TraceEvent eventData)
        {
            throw new NotImplementedException();
        }
    }
}
