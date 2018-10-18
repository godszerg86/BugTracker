using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }

        //relations
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }

        //local propeties
        public string FilePath { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }


    }
}