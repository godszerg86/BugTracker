using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            TypeCss = new Dictionary<int, string>() {
                {1,"<i class=\"fab fa-angular fa-5x\"></i>" },
                {2,"<i class=\"fab fa-vuejs fa-5x\"></i>" },
                {3,"<i class=\"fab fa-react fa-5x\"></i>" },
                {4,"<i class=\"fab fa-windows fa-5x\"></i>" },
                {5,"<i class=\"fab fa-node fa-5x\"></i>" },
            };

            TypeColor = new Dictionary<int, string>() {
                {1,"danger" },
                {2,"succes" },
                {3,"primary" },
                {4,"info" },
                {5,"warning" },
            };
        }

        public int Id { get; set; }

        //foreign keys connections 
        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }


        public int ProjectTypeId { get; set; }
        public virtual ProjectType ProjectType { get; set; }
        //many-to-many relations
        public virtual ICollection<ApplicationUser> AssignedDevelopers { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }

        // local properties
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        [Required(ErrorMessage ="Name field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description field is required.")]
        public string Description { get; set; }

        public Dictionary<int,string> TypeCss { get; private set; }
        public Dictionary<int, string> TypeColor { get; private set; }


    }
}