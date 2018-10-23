using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models.ActionFilters
{
    public class CheckTicketOwnFilter : ActionFilterAttribute
    {
        ApplicationDbContext db { get; set; }
        UserHelper UserHelper { get; set; }

      


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            db = new ApplicationDbContext();
            UserHelper = new UserHelper(db);
            var userDB = UserHelper.GetUserById(filterContext.HttpContext.User.Identity.GetUserId());
            int ticketId = (int)filterContext.ActionParameters["id"];
            var ticket = db.Tickets.FirstOrDefault(t => t.Id == ticketId);



            if ((userDB.ProjectsManage.Any(p => p.Id == ticket.ProjectId) && filterContext.HttpContext.User.IsInRole("Project Manager"))
                || ((ticket.DeveloperId == userDB.Id) && filterContext.HttpContext.User.IsInRole("Developer")))
            {
                return;

            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                                   new System.Web.Routing.RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } }
                               );
            }
        }
    }
}