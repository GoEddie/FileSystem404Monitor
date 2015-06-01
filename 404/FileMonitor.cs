using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Diagnostics.Tracing;
using _404.Aggregators;
using _404.Etw;
using _404.ProcessStore;

namespace _404
{
    public class FileMonitor
    {
        private const long Success = 0x0;
        private readonly Action<MissingFile, SearchEvent> _addEvent;
        private readonly Action<MissingFile> _displayFile;
        private readonly Dictionary<string, MissingFile> _files = new Dictionary<string, MissingFile>();
        private readonly object _lock = new object();
        private readonly ProcessNameStore _nameStore;
        private readonly EtwEventProvider _provider;
        private readonly Action<MissingFile> _removeFile;
        private bool _continue;

        public FileMonitor(Action<MissingFile> displayFile, Action<MissingFile> removeFile,
            Action<MissingFile, SearchEvent> addEvent)
        {
            _displayFile = displayFile;
            _removeFile = removeFile;
            _addEvent = addEvent;

            var fileEventAggregator = new FileIoAggregator(FileEventAvailable);
            _provider = new EtwEventProvider("File404Monitor", "Microsoft-Windows-Kernel-File", 0x10c0,
                fileEventAggregator);
            _nameStore = new ProcessNameStore();
        }

        private void FileEventAvailable(FileIoEvent fileIo)
        {
            if (fileIo.Status == Success)
            {
                //do we have a not found which is now found?
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
            searchEvent.PID = fileIo.PID;
            searchEvent.EventTime = fileIo.Time;

            _addEvent(missingFile, searchEvent);
        }

        public void Start()
        {
            if (!(TraceEventSession.IsElevated() ?? false))
            {
                MessageBox.Show("You must run this elevated to start a trace");
                return;
            }

            _provider.Start();
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