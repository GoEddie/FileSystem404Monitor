using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _404
{
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

        public FileMonitor(Action<MissingFile> displayFile)
        {
            _displayFile = displayFile;
        }

        private bool _continue = false;
        private readonly object _lock = new object();

        public void Start()
        {
            lock (_lock)
            {
                if (_continue)
                    return;

                _continue = true;
            }
            
            while (_continue && Sleep(1000))
                    _displayFile(new MissingFile()
                {
                    LastLookupAttempt = DateTime.Now,
                    Lookups =
                        new List<SearchEvent>()
                        {
                            new SearchEvent() {Path = "c:\\dssdlksdlksldks", Result = 0xc000034},
                            new SearchEvent() {Path = "c:\\dssdlksdlksldks", Result = 0xc000034},
                            new SearchEvent() {Path = "c:\\dssdlksdlksldks", Result = 0xc000034},
                            new SearchEvent() {Path = "c:\\dssdlksdlksldks", Result = 0xc000034}
                        },
                    Name = "Blah",
                    PID = 100,
                    TID = 1022,
                    Process = "dsdssds"
                });



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
