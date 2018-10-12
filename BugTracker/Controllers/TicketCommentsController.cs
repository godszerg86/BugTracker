using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db { get; set; }

        private UserHelper UserHelper { get; set; }

        public TicketCommentsController()
        {
            db = new ApplicationDbContext();
            UserHelper = new UserHelper(db);
        }
        // GET: TicketComments
        public ActionResult Index()
        {
            var ticketComments = db.TicketComments.Include(t => t.Author).Include(t => t.Ticket);
            return View(ticketComments.ToList());
        }

        // GET: TicketComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // GET: TicketComments/Create
        public ActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "DisplayName");
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "AuthorId");
            return View();
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,Body")] TicketComment ticketComment, int ProjectId)
        {
            if (ModelState.IsValid)
            {
                var userDB = UserHelper.GetUserById(User.Identity.GetUserId());
                var ticketDb = db.Tickets.FirstOrDefault(t => t.Id == ticketComment.TicketId);

                if ((User.Identity.IsAuthenticated && User.IsInRole("Admin"))
                    || (User.Identity.IsAuthenticated && User.IsInRole("Project Manager") && userDB.ProjectsManage.Any(p => p.Id == ProjectId))
                    || (User.Identity.IsAuthenticated && User.IsInRole("Submitter") && ticketDb.AuthorId == userDB.Id)
                    || (User.Identity.IsAuthenticated && User.IsInRole("Developer") && ticketDb.Developer.Id == userDB.Id))
                {

                    ticketComment.Created = DateTime.Now;
                    ticketComment.AuthorId = User.Identity.GetUserId();
                    db.TicketComments.Add(ticketComment);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });

                }

                return View("NoAccess", ticketComment.TicketId);

            }

            return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
        }

        // GET: TicketComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "DisplayName", ticketComment.AuthorId);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "AuthorId", ticketComment.TicketId);
            return View(ticketComment);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Body,Created,Updated,AuthorId,TicketId")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "DisplayName", ticketComment.AuthorId);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "AuthorId", ticketComment.TicketId);
            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment ticketComment = db.TicketComments.Find(id);
            db.TicketComments.Remove(ticketComment);
            db.SaveChanges();
            return RedirectToAction("Index");
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
