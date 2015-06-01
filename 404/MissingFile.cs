using System;
using System.Collections.ObjectModel;

namespace _404
{
    public class MissingFile
    {
        public string Name { get; set; }
        public DateTime LastLookupAttempt { get; set; }
        public string Process { get; set; }
        public int PID { get; set; }
        public int TID { get; set; }
        public ObservableCollection<SearchEvent> Lookups { get; set; }
    }
}