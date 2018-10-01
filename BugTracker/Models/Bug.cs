using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Bug
    {
     
        public int Id { get; set; }

        // foreign keys connections
        public int AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public int StatusId { get; set; }
        public virtual Status Status { get; set; }

        public int PrevBodyId { get; set; }
        public virtual BugBodies PrevBody { get; set; }

        //local properties
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public enum StatusCode { OPEN = 1, CLOSED = 2, REVIEW = 3 };


        public string Body { get; set; }
        public string Subject { get; set; }


    }
}