using System;

namespace _404
{
    public class SearchEvent
    {
        public int PID { get; set; }
        public string Path { get; set; }
        public int Result { get; set; }
        public DateTime EventTime { get; set; }
        public string Process { get; set; }
    }
}