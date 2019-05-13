using System;
using System.Collections.Generic;
using System.Text;

namespace Prioritize.Models
{
    public class AppConfig 
    {
        public string PrioritizeItConnectionString { get; set; }
        public bool DebugEmail { get; set; }
        public List<string> DebugEmailTo { get; set; }
        public string ServerName { get; set; }
        public int ServerPort { get; set; }
        public bool EnableSSL { get; set; }
        public string MailFromAddress { get; set; }
        public string MailFromName { get; set; }
        public List<string> OnRefreshMailList { get; set; }
        public string Environment { get; set; }
        public string ErrorEmailAddress { get; set; }
        public List<string> NewItemCreatedEmail { get; set; }
        public string HostUrl { get; set; }
    }
}
