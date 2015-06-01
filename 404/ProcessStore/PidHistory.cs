using System;
using System.Collections.Generic;
using System.Linq;

namespace _404.ProcessStore
{
    internal class PidHistory
    {
        private readonly List<ProcessName> _processes = new List<ProcessName>();
        public int Pid;

        public PidHistory(ProcessName name, int pid)
        {
            _processes.Add(name);
            Pid = pid;
        }

        public void AddProcessName(ProcessName name)
        {
            if (_processes.Count > 0)
            {
                SetEndTimeOnLastProcess(name.ValidFrom);
            }

            _processes.Add(name);
        }

        private void SetEndTimeOnLastProcess(DateTime endTime)
        {
            _processes.OrderByDescending(p => p.ValidTo).First().ValidTo = endTime.Subtract(new TimeSpan(0, 0, 0, 1));
        }

        public string GetName(DateTime at)
        {
            foreach (var name in _processes)
            {
                if (name.ValidFrom <= at && name.ValidTo >= at)
                {
                    return name.Name;
                }
            }

            return "Not Found";
        }
    }
}