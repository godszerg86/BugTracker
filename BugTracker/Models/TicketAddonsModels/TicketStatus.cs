using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.TicketAddonsModels
{
    public class TicketStatus
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string HexColor { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}