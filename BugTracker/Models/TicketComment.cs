using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        public string Body { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        
        
        //relations
        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}