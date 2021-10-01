using System;

namespace signalrCore.Entities
{
    public class LogChange
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string InfoForm { get; set; }

        public string TableName { get; set; }

        public string FieldName { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string UserName { get; set; }

        public int IdEntity { get; set; }

        public string Detail { get; set; }
    }
}
