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

        //one-to-many
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        //local propeties
        public string FilePath { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }


    }
}