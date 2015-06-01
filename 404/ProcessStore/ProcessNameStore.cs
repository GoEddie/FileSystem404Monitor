using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using _404.Aggregators;
using _404.Etw;

namespace _404.ProcessStore
{
    public class ProcessNameStore
    {
        private readonly Dictionary<int, PidHistory> _pids = new Dictionary<int, PidHistory>();
        private ProcessAggregator _aggregator;
        private EtwEventProvider _provider;

        public ProcessNameStore()
        {
            StartTrace();

            GetInitialSnapshot();
        }

        private void StartTrace()
        {
            _aggregator = new ProcessAggregator(ProcessEvent);
            _provider = new EtwEventProvider("404MonitorProcessLookups", "Microsoft-Windows-Kernel-Process", 0x10,
                _aggregator);
            ThreadPool.QueueUserWorkItem(StartProvider);
        }

        private void StartProvider(object state)
        {
            _provider.Start();
        }

        private void ProcessEvent(ProcessStartEvent process)
        {
            AddProcessHistory(process.Pid, process.Name, process.StartTime, DateTime.MaxValue);
        }

        private void GetInitialSnapshot()
        {
            foreach (var process in Process.GetProcesses())
            {
                AddProcessHistory(process.Id, process.ProcessName, DateTime.MinValue, DateTime.MaxValue);
            }
        }

        private void AddProcessHistory(int pid, string name, DateTime from, DateTime to)
        {
            if (!_pids.ContainsKey(pid))
            {
                _pids.Add(pid, new PidHistory(new ProcessName(name, from, to), pid));
                return;
            }

            _pids[pid] = new PidHistory(new ProcessName(name, from, to), pid);
        }

        public string ProcessName(int pid, DateTime eventTime)
        {
            if (!_pids.ContainsKey(pid))
            {
                return "Not Known";
            }

            return _pids[pid].GetName(eventTime);
        }
    }
}