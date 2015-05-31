using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using _404.Aggregators;
using _404.Etw;
using _404.ProcessStore;

namespace _404
{
    public class FileMonitor
    {
        private readonly Dictionary<string, MissingFile> _files = new Dictionary<string, MissingFile>();
        private readonly Action<MissingFile> _displayFile;
        private readonly Action<MissingFile> _removeFile;
        private readonly EtwEventProvider _provider;
        private readonly FileIOAggregator _fileEventAggregator;
        private readonly ProcessNameStore _nameStore;
        public FileMonitor(Action<MissingFile> displayFile, Action<MissingFile> removeFile)
        {
            _displayFile = displayFile;
            _removeFile = removeFile;

            _fileEventAggregator = new FileIOAggregator(FileEventAvailable);
            _provider = new EtwEventProvider("File404Monitor", "Microsoft-Windows-Kernel-File", 0x10c0, _fileEventAggregator );
            _nameStore = new ProcessNameStore();
            
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
            searchEvent.Process = _nameStore.ProcessName(fileIo.PID, fileIo.Time);
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