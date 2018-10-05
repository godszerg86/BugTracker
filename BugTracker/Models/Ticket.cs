using System;
using System.Collections.Generic;
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

        public string DeveloperId { get; set; }
        public virtual ApplicationUser Developer { get; set; }

        //local proprties
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }




    }
}