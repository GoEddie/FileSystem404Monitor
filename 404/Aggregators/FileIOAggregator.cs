using System;   
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diagnostics.Tracing;
using _404.Etw;

namespace _404.Aggregators
{
    public class FileIOAggregator : EventAggregator
    {
        private readonly Action<FileIOEvent> _completed;
        private readonly Dictionary<string, FileIOEvent> _operations = new Dictionary<string, FileIOEvent>();

        public FileIOAggregator(Action<FileIOEvent> completed)
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

                if (eventData.TaskName == "OperationEnd")
                {
                    if (_operations.ContainsKey(irp))
                    {
                        var fileIo = _operations[irp];
                        _operations.Remove(irp);
                        var status = eventData.PayloadByName("Status") as int?;
                        fileIo.Status = status ?? -1;
                        fileIo.Name = Path.GetFileName(fileIo.FullPath);
                        _completed(fileIo);
                    }

                    return;
                }
                else
                {

                    var fileIo = new FileIOEvent(eventData);
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
