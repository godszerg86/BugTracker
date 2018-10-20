using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models.classes
{
    public class CreateTicketListModel
    {
        public int ProjectId { get; set; }

       
        public string Title { get; set; }
        public string Description { get; set; }

        public SelectList TicketType { get; set; }

    }
}