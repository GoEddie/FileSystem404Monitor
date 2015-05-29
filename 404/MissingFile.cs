using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Diagnostics.Tracing;
using _404.Aggregators;
using _404.Etw;

namespace _404
{

    public class FileIOEvent
    {
        public string FullPath;
        public long Status;
        public DateTime Time;
        public int PID;
        public string Process;
        public string Irp;
        public string RequestType;

        public FileIOEvent(TraceEvent eventData)
        {
            Irp = eventData.PayloadByName("Irp").ToString();
            Process = eventData.ProcessName;
            PID = eventData.ProcessID;
            Time = eventData.TimeStamp;
            FullPath = eventData.PayloadByName("FileName") as string;
            RequestType = eventData.TaskName;
        }
    }

    public class SearchEvent
    {
        public string Path { get; set; }
        public long Result { get; set; }
    }

    public class MissingFile
    {
        public string Name { get; set; }
        public DateTime LastLookupAttempt { get; set; }
        public string Process { get; set; }
        public int PID { get; set; }
        public int TID { get; set; }

        public List<SearchEvent> Lookups { get; set; }
    }

    public class FileMonitor
    {
        private readonly Action<MissingFile> _displayFile;
        private readonly EtwEventProvider _provider;
        private readonly FileIOAggregator _fileEventAggregator;
        public FileMonitor(Action<MissingFile> displayFile)
        {
            _displayFile = displayFile;

            _fileEventAggregator = new FileIOAggregator(FileEventAvailable);
            _provider = new EtwEventProvider("File404Monitor", "Microsoft-Windows-Kernel-File", 0x10c0, _fileEventAggregator );
        }

        private void FileEventAvailable(FileIOEvent fileIo)
        {
            Console.WriteLine("FileIO: {0} {1}", fileIo.FullPath, fileIo.RequestType);
        }

        private bool _continue = false;
        private readonly object _lock = new object();

        public void Start()
        {
            
               _provider.Start();
            
            //lock (_lock)
            //{
            //    if (_continue)
            //        return;

            //    _continue = true;
            //}

            //while (_continue && Sleep(1000))
            //        _displayFile(new MissingFile()
            //    {
            //        LastLookupAttempt = DateTime.Now,
            //        Lookups =
            //            new List<SearchEvent>()
            //            {
            //                new SearchEvent() {Path = "c:\\dssdlksdlksldks", Result = 0xc000034},
            //                new SearchEvent() {Path = "c:\\dssdlksdlksldks", Result = 0xc000034},
            //                new SearchEvent() {Path = "c:\\dssdlksdlksldks", Result = 0xc000034},
            //                new SearchEvent() {Path = "c:\\dssdlksdlksldks", Result = 0xc000034}
            //            },
            //        Name = "Blah",
            //        PID = 100,
            //        TID = 1022,
            //        Process = "dsdssds"
            //    });



        }

        private bool Sleep(int i)
        {
            Thread.Sleep(i);
            return true;
        }
    

        public void Stop()
        {
            lock (_lock)
            {
                _continue = false;
            }

        }

    }


}
