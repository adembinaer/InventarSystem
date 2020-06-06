using System;
using System.Collections.Generic;
using DuplicateCheckerLib;

namespace LogSystem
{
    public class LoggingEntry : IEntity
    {
        public int Id { get; set; }
        public string Pod { get; set; }
        public string Hostname { get; set; }
        public int Severity { get; set; }
        public string Location { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }

        public override bool Equals(object value)
        {
            var loggingEntry = value as LoggingEntry;
            return loggingEntry != null && Severity == loggingEntry.Severity && Message== loggingEntry.Message;
        }

        public override int GetHashCode()
        {
            return (152123 * 221 + Severity.GetHashCode()) * -15211347 +
                   EqualityComparer<string>.Default.GetHashCode(Message);
        }
    }
}

