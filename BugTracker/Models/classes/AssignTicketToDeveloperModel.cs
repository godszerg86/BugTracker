using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models.classes
{
    public class AssignTicketToDeveloperModel
    {
        public int TicketId { get; set; }
        public ICollection<ApplicationUser> Developers { get; set; }
        public SelectList DevList { get; set; }
        public ApplicationUser SelectedDev { get; set; }
        public string TicketTitle { get; set; }
        public int ProjectId { get; set; }

    }
}
