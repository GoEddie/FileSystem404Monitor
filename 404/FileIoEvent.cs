using System;
using Diagnostics.Tracing;

namespace _404
{
    public class FileIoEvent
    {
        public string FullPath;
        public string Irp;
        public string Name;
        public int PID;
        public string Process;
        public string RequestType;
        public int Status;
        public DateTime Time;

        public FileIoEvent(TraceEvent eventData)
        {
            Irp = eventData.PayloadByName("Irp").ToString();
            Process = eventData.ProcessName;
            PID = eventData.ProcessID;
            Time = eventData.TimeStamp;
            FullPath = eventData.PayloadByName("FileName") as string;
            RequestType = eventData.TaskName;
        }
    }
}