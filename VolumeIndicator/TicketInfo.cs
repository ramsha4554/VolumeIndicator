using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeIndicator
{
    internal class TicketInfo
    {
        public string TicketId { get; set; }
        public int Limit { get; set; }

       public TicketInfo(string ticketId, int limit)
        {
            TicketId = ticketId;
            Limit = limit;
            
        }

    }
}
