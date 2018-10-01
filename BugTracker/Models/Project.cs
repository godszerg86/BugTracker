using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Project
    {
        public Project()
        {
            Bugs = new List<Bug>();
        }

        public int Id { get; set; }

        //foreign keys connections
        public int AuthroId { get; set; }
        public virtual ApplicationUser Author {get; set;}


        // local properties
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Bug> Bugs { get; set; }



    }
}