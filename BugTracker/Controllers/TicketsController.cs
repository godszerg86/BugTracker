using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.Author).Include(t => t.Developer).Include(t => t.Project);
            return View(tickets.ToList());
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
        public ActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "DisplayName");
            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "DisplayName");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "AuthorId");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectId,AuthorId,DeveloperId,Title,Description,Created,Updated")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "DisplayName", ticket.AuthorId);
            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "DisplayName", ticket.DeveloperId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "AuthorId", ticket.ProjectId);
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
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "DisplayName", ticket.AuthorId);
            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "DisplayName", ticket.DeveloperId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "AuthorId", ticket.ProjectId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,AuthorId,DeveloperId,Title,Description,Created,Updated")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "DisplayName", ticket.AuthorId);
            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "DisplayName", ticket.DeveloperId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "AuthorId", ticket.ProjectId);
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
