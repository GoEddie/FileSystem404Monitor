using System;

namespace _404.ProcessStore
{
    internal class ProcessName
    {
        public string Name;
        public DateTime ValidFrom;
        public DateTime ValidTo;

        public ProcessName(string name, DateTime from, DateTime to)
        {
            Name = name;
            ValidFrom = from;
            ValidTo = to;
        }
    }
}