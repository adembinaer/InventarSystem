using System;

namespace LogSystem
{
    public class LoggingEntry
    {
        public int Id { get; set; }
        public string Pod { get; set; }
        public string Hostname { get; set; }
        public int Severity { get; set; }
        public string Location { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }
}

