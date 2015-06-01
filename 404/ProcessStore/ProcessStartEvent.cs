using System;

namespace _404.ProcessStore
{
    internal class ProcessStartEvent
    {
        public string Name;
        public int Pid;
        public DateTime StartTime;
    }
}