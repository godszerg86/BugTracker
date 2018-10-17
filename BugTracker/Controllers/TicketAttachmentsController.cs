using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Models.classes;
using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    public class TicketAttachmentsController : Controller
    {
        private ApplicationDbContext db { get; set; }
        private UserHelper UserHelper { get; set; }

        public TicketAttachmentsController()
        {
            db = new ApplicationDbContext();
            UserHelper = new UserHelper(db);
        }


        // GET: TicketAttachments
        public ActionResult Index()
        {
            var ticketAttachments = db.TicketAttachments.Include(t => t.Ticket);
            return View(ticketAttachments.ToList());
        }

        // GET: TicketAttachments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Create
        [Authorize(Roles = "Admin,Project Manager,Submitter,Developer")]
        public ActionResult Create(int ticketId, int projectId)
        {

            var userDB = UserHelper.GetUserById(User.Identity.GetUserId());
            var ticketDB = db.Tickets.FirstOrDefault(t => t.Id == ticketId);

            if (User.IsInRole("Admin") ||
                (User.IsInRole("Project Manager") && userDB.ProjectsManage.Any(p => p.Id == projectId)) ||
                (User.IsInRole("Developer") && ticketDB.DeveloperId == userDB.Id) ||
                (User.IsInRole("Submitter") && userDB.CreatedTickets.Any(t => t.Id == ticketId)))
            {
                ViewBag.ProjectId = projectId;
                return View(ticketId);
            }
            else
            {
                return RedirectToAction("Details", "Tickets", new { id = ticketId });
            }


        }

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FileBase,FileDescription")] List<TicketAttachmentViewModel> ticketAttachments, int TicketId, int ProjectId)
        {
            if (ModelState.IsValid)
            {
                var dateTimeNow = DateTime.Now;
                var userDB = UserHelper.GetUserById(User.Identity.GetUserId());
                var ticketDB = db.Tickets.FirstOrDefault(t => t.Id == TicketId);

                if (User.IsInRole("Admin") ||
                (User.IsInRole("Project Manager") && userDB.ProjectsManage.Any(p => p.Id == ProjectId)) ||
                (User.IsInRole("Developer") && ticketDB.DeveloperId == userDB.Id) ||
                (User.IsInRole("Submitter") && userDB.CreatedTickets.Any(t => t.Id == TicketId)))
                {
                    foreach (var attach in ticketAttachments)
                    {

                        if (attach.FileBase != null)
                        {

                            var attachmentDB = new TicketAttachment();
                            attachmentDB.AuthorId = User.Identity.GetUserId();
                            attachmentDB.Created = dateTimeNow;
                            attachmentDB.Description = attach.FileDescription;
                            attachmentDB.TicketId = TicketId;
                            var hash = attach.GetHashCode();
                            attachmentDB.Name = SlugConverter.URLFriendly(Path.GetFileNameWithoutExtension(attach.FileBase.FileName)) + "-" + hash.ToString() + Path.GetExtension(attach.FileBase.FileName);
                            var content = Server.MapPath("~/uploads/tickets/");
                            var path = Path.Combine(content, Convert.ToString(TicketId));
                            Directory.CreateDirectory(path);

                            attach.FileBase.SaveAs(Path.Combine(Server.MapPath("~/uploads/tickets/" + TicketId + "/"), attachmentDB.Name));
                            attachmentDB.FilePath = "/uploads/tickets/" + TicketId + "/" + attachmentDB.Name;
                            db.TicketAttachments.Add(attachmentDB);
                        }
                        else
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                    }


                    db.SaveChanges();
                    return RedirectToAction("Details", "Tickets", new { id = TicketId });
                }
            }

            return RedirectToAction("Details", "Tickets", new { id = TicketId });
        }

        // GET: TicketAttachments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "AuthorId", ticketAttachment.TicketId);
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,FilePath,Created,Updated,Name,Description")] TicketAttachment ticketAttachment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketAttachment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "AuthorId", ticketAttachment.TicketId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(ticketAttachment);
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
