using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.classes
{
    public class TicketAttachmentViewModel
    {

        public HttpPostedFileBase FileBase { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }

    }
}