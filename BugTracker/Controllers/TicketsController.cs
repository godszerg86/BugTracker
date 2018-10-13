using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Models.classes;
using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db { get; set; }
        private UserHelper UserHelper { get; set; }

        public TicketsController()
        {
            db = new ApplicationDbContext();
            UserHelper = new UserHelper(db);

        }


        // GET: Tickets
        public ActionResult Index(int? page, int? pageSizeIn, string sortedByTitle, string query)
        {
            ViewBag.Controller = "Index";
            int pageSize = (pageSizeIn ?? 2); // display three blog posts at a time on this page
            int pageNumber = (page ?? 1);
            List<Ticket> tickets;

            if (string.IsNullOrEmpty(query) || string.IsNullOrWhiteSpace(query))
            {
                tickets = db.Tickets.Include(t => t.Author).Include(t => t.Developer).Include(t => t.Project).ToList();
            }
            else
            {
                tickets = db.Tickets.Where(
                                            t => t.Title.ToLower()
                                            .Contains(query.ToLower()) ||
                                            t.Description.ToLower()
                                            .Contains(query.ToLower()))
                                            .Include(t => t.Author).Include(t => t.Developer).Include(t => t.Project).ToList();
                ViewBag.searchText = query;
            }

            if (sortedByTitle == "a-z")
            {
                ViewBag.Sorted = "a-z";
                return View(tickets.OrderBy(t => t.Title).ToPagedList(pageNumber, pageSize));
            }
            else if (sortedByTitle == "z-a")
            {
                ViewBag.Sorted = "z-a";
                return View(tickets.OrderByDescending(t => t.Title).ToPagedList(pageNumber, pageSize));
            }

            return View(tickets.ToPagedList(pageNumber, pageSize));
        }

        // GET: Tickets of current user
        public ActionResult MyTickets(int? page, int? pageSizeIn)
        {
            ViewBag.Controller = "MyTickets";
            string userID = User.Identity.GetUserId();
            int pageSize = (pageSizeIn ?? 10); // display three blog posts at a time on this page
            int pageNumber = (page ?? 1);
            if (User.IsInRole("Developer"))
            {
                var tickets = db.Tickets.Where(t => t.DeveloperId == userID).Include(t => t.Author).Include(t => t.Developer).Include(t => t.Project).ToList();
                return View("Index", tickets.ToPagedList(pageNumber, pageSize));
            }

            if (User.IsInRole("Submitter"))
            {

                var tickets = db.Tickets.Where(t => t.AuthorId == userID).Include(t => t.Author).Include(t => t.Developer).Include(t => t.Project).ToList();
                return View("Index", tickets.ToPagedList(pageNumber, pageSize));
            }

            return View("Index");
        }

        public ActionResult MyProjectsTickets(int? page, int? pageSizeIn)
        {
            ViewBag.Controller = "MyProjectsTickets";
            string userID = User.Identity.GetUserId();
            int pageSize = (pageSizeIn ?? 10); // display three blog posts at a time on this page
            int pageNumber = (page ?? 1);
            var userDB = UserHelper.GetUserById(userID);
            var tickets = db.Tickets.Where(t => t.Project.AssignedDevelopers.Any(u => u.Id == userID)).Include(t => t.Author).Include(t => t.Developer).Include(t => t.Project).ToList();
            return View("Index", tickets.ToPagedList(pageNumber, pageSize));
        }


        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create(int projectId)
        {
            var ticketModel = new CreateTicketListModel();
            ticketModel.ProjectId = projectId;
            return View(ticketModel);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Submitter")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectId,Title,Description")] Ticket ticket, TicketAttachment ticketAttachment)
        { //TODO:
            var projectDB = db.Projects.Find(ticket.ProjectId);
            if (projectDB == null)
            {
                return HttpNotFound();
            }


            var userAssignedProjectsDB = UserHelper.GetAllProjectsAssignedToUser(User.Identity.GetUserId());

            if (!userAssignedProjectsDB.Any(proj => proj.Id == ticket.ProjectId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {

                ticket.Created = DateTime.Now;
                ticket.AuthorId = User.Identity.GetUserId();
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index", "Projects");
            }

    ;
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var ticketDB = db.Tickets.FirstOrDefault(t => t.Id == ticket.Id);
                ticketDB.Title = ticket.Title;
                ticketDB.Description = ticket.Description;
                ticketDB.Updated = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index", "Projects");
            }
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //GET: Assign developer to the ticket
        public ActionResult AssignDeveloper(int id)
        {
            var viewModel = new AssignTicketToDeveloperModel();
            viewModel.TicketId = id;
            var ticket = db.Tickets.FirstOrDefault(t => t.Id == id);
            viewModel.TicketTitle = ticket.Title;
            viewModel.Developers = UserHelper.GetUsersInRole("Developer");
            if (ticket.Developer != null)
            {
                viewModel.DevList = new SelectList(viewModel.Developers, "Id", "DisplayName", ticket.Developer.Id);
            }
            else
            {
                viewModel.DevList = new SelectList(viewModel.Developers, "Id", "DisplayName");
            }
            viewModel.ProjectId = ticket.ProjectId;
            return View(viewModel);
        }


        //POST: Assgin developer to ticjet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignDeveloper([Bind(Include = "TicketId,SelectedDevId,ProjectId")] AssignTicketToDeveloperModel model)
        {

            var ticket = db.Tickets.Find(model.TicketId);
            ticket.DeveloperId = model.SelectedDevId;
            db.SaveChanges();
            return RedirectToAction("Details", "Projects", new { id = model.ProjectId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
