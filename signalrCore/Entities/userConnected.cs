using System;
using System.Collections.Generic;

namespace signalrCore.Entities
{
    public class userConnected
    {
        public string companyId { get; set; }
        public string userId { get; set; }
        public string connectionId { get; set; }
        public string appId { get; set; }
        public DateTime dateLog { get; set; }
        public int deviceType { get; set; }
        public string deviceDesc { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string url { get; set; }
    }
}
