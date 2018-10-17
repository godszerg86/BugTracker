using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        //many-to-manu relations

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }


        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }

        public string AssigneeId { get; set; }
        public virtual ApplicationUser Assignee { get; set; }

        public string DeveloperId { get; set; }
        public virtual ApplicationUser Developer { get; set; }


        public virtual ICollection<TicketComment> Comments { get; set; }
        public virtual ICollection<TicketAttachment> Attachments { get; set; }


        //local proprties
        [Required(ErrorMessage = "Ticket title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Ticket description is required")]
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }




    }
}