using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubsReceiver.Wpf
{
    public class EventData
    {
        public string OperationName { get; set; }
        public string Status { get; set; }
        public string Time { get; set; }
        public string TimeStamp { get; set; }
        public string Subscription { get; set; }
        public string InitiatedBy { get; set; }
    }
}
