using System;
using System.Collections.Generic;
using System.IO;
using Diagnostics.Tracing;

namespace _404.Aggregators
{
    public class FileIoAggregator : EventAggregator
    {
        private readonly Action<FileIoEvent> _completed;
        private readonly Dictionary<string, FileIoEvent> _operations = new Dictionary<string, FileIoEvent>();
        private const string operationEnd = "OperationEnd";
        private const string payloadStatus = "Status";

        public FileIoAggregator(Action<FileIoEvent> completed)
        {
            _completed = completed;
        }

        public override void TraceEventAvailable(TraceEvent eventData)
        {
            try
            {
                var irp = eventData.PayloadByName("Irp").ToString();

                if (String.IsNullOrEmpty(irp))
                    return;

                
                if (eventData.TaskName == operationEnd)
                {
                    if (_operations.ContainsKey(irp))
                    {
                        var fileIo = _operations[irp];
                        _operations.Remove(irp);
                        
                        var status = eventData.PayloadByName(payloadStatus) as int?;
                        fileIo.Status = status ?? -1;
                        fileIo.Name = Path.GetFileName(fileIo.FullPath);
                        _completed(fileIo);
                    }
                }
                else
                {
                    var fileIo = new FileIoEvent(eventData);
                    _operations[fileIo.Irp] = fileIo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}