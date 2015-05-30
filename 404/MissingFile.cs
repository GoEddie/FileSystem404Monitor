using System;
using System.Collections.Generic;
using System.IO;
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
        public string Name;
        public string FullPath;
        public int Status;
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
        public MissingFile()
        {
            Lookups = new List<SearchEvent>();
        }
        public string Name { get; set; }
        public DateTime LastLookupAttempt { get; set; }
        public string Process { get; set; }
        public int PID { get; set; }
        public int TID { get; set; }

        public List<SearchEvent> Lookups { get; set; }
    }

    public class FileMonitor
    {
        private readonly Dictionary<string, MissingFile> _files = new Dictionary<string, MissingFile>();
        private readonly Action<MissingFile> _displayFile;
        private readonly Action<MissingFile> _removeFile;
        private readonly EtwEventProvider _provider;
        private readonly FileIOAggregator _fileEventAggregator;
        public FileMonitor(Action<MissingFile> displayFile, Action<MissingFile> removeFile)
        {
            _displayFile = displayFile;
            _removeFile = removeFile;

            _fileEventAggregator = new FileIOAggregator(FileEventAvailable);
            _provider = new EtwEventProvider("File404Monitor", "Microsoft-Windows-Kernel-File", 0x10c0, _fileEventAggregator );
        }

        private void FileEventAvailable(FileIOEvent fileIo)
        {
            //Console.WriteLine("FileIO: {0} {1}", fileIo.FullPath, fileIo.RequestType);

            if (fileIo.Status == Success)
            {   //do we have a not found which is now found?
                if (_files.ContainsKey(fileIo.Name))
                {
                    var file = _files[fileIo.Name];
                    _removeFile(file);
                }

                return;
            }

            if (!_files.ContainsKey(fileIo.Name))
            {
                var file = new MissingFile();
                file.Name = fileIo.Name;
                file.PID = fileIo.PID;
                file.Process = fileIo.Process;
                
                _files.Add(fileIo.Name, file);

                _displayFile(file);
            }

            var missingFile = _files[fileIo.Name];
            missingFile.LastLookupAttempt = fileIo.Time;
            var searchEvent = new SearchEvent();
            searchEvent.Path = Path.GetDirectoryName(fileIo.FullPath);
            searchEvent.Result = fileIo.Status;

            missingFile.Lookups.Insert(0, searchEvent);

        }

        private bool _continue = false;
        private readonly object _lock = new object();
        private const long Success = 0x0;

        public void Start()
        {
            
               _provider.Start();
            

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
