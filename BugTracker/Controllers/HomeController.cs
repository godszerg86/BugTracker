using BugTracker.Models;
using Highsoft.Web.Mvc.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {

        private ApplicationDbContext db { get; set; }

        public HomeController()
        {
            db = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            List<PieSeriesData> pieProjects = new List<PieSeriesData>();
            List<PieSeriesData> ticketTypes = new List<PieSeriesData>();
            List<PieSeriesData> ticketStatus = new List<PieSeriesData>();
            List<PieSeriesData> ticketPriority = new List<PieSeriesData>();

            var projectTypesDB = db.ProjectTypes.ToList();
            var tickesTypesDB = db.TicketType.ToList();
            var tickesStatusDB = db.TicketStatus.ToList();
            var tickesPriorityDB = db.TicketPriority.ToList();


            foreach (var type in projectTypesDB)
            {
                int dbCount = db.Projects.Where(p => p.ProjectTypeId == type.Id).Count();
                if (dbCount != 0)
                {
                    pieProjects.Add(new PieSeriesData { Name = type.Type, Y = dbCount, Color = type.HexColor });

                }
            }

            foreach (var type in tickesTypesDB)
            {
                int dbCount = db.Tickets.Where(p => p.TicketTypeId == type.Id).Count();
                if (dbCount != 0)
                {
                    ticketTypes.Add(new PieSeriesData { Name = type.Name, Y = dbCount, Color = type.HexColor });
                }
            }

            foreach (var status in tickesStatusDB)
            {
                int dbCount = db.Tickets.Where(p => p.TicketStatusId == status.Id).Count();
                if (dbCount != 0)
                {
                    ticketStatus.Add(new PieSeriesData { Name = status.Name, Y = dbCount, Color = status.HexColor });
                }
            }

            foreach (var priority in tickesPriorityDB)
            {
                int dbCount = db.Tickets.Where(p => p.TicketPriorityId == priority.Id).Count();
                if (dbCount != 0)
                {
                    ticketPriority.Add(new PieSeriesData { Name = priority.Name, Y = dbCount, Color = priority.HexColor });
                }
            }

           

            ViewData["pieProjects"] = pieProjects;
            ViewData["pieTicketStatus"] = ticketStatus;
            ViewData["pieTicketPriority"] = ticketPriority;
            ViewData["pieTicketType"] = ticketTypes;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}