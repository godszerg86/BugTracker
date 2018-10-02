using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Project
    {
        public Project()
        {
            AssignedDevelopers = new HashSet<ApplicationUser>();
        }

        public int Id { get; set; }

        //foreign keys connections
        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }

        public virtual ICollection<ApplicationUser> AssignedDevelopers { get; set; }

        public int ProjectTypeId { get; set; }
        public virtual ProjectType ProjectType { get; set; }

        // local properties
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }




    }
}