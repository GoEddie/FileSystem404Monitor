using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Diagnostics.Tracing;

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

        public string Process { get; set; }
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
}
