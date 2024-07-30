using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.MessageDelivery.Common.Models
{
    public class MessagesOnHistoryModel
    {
        public List<MessagesOnTopic> Messages { get; set; }   
    }

    public class MessagesOnTopic
    {
        public string MessageId { get; set; }
        public string MessageText { get; set; }
        public DateTime EnqueuedTime { get; set; }
    }
}
