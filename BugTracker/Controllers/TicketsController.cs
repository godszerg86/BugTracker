using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using BugTracker.Hubs;
using BugTracker.Models;
using BugTracker.Models.ActionFilters;
using BugTracker.Models.classes;
using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
        [Authorize]
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
        [Authorize(Roles = "Submitter,Developer")]
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

            return View("NoAccess");
        }
        [Authorize(Roles = "Project Manager,Developer")]
        public ActionResult MyProjectsTickets(int? page, int? pageSizeIn)
        {
            ViewBag.Controller = "MyProjectsTickets";

            int pageSize = (pageSizeIn ?? 10); // display three blog posts at a time on this page
            int pageNumber = (page ?? 1);


            if ((User.IsInRole("Project Manager") && User.Identity.IsAuthenticated)
                || (User.IsInRole("Developer") && User.Identity.IsAuthenticated))
            {
                var userDB = UserHelper.GetUserById(User.Identity.GetUserId());
                var projectsManageDB = userDB.ProjectsManage.ToList();
                List<Ticket> tickets = new List<Ticket>();
                foreach (var proj in projectsManageDB)
                {
                    foreach (var ticket in proj.Tickets)
                    {
                        tickets.Add(ticket);
                    }
                }
                return View("Index", tickets.ToPagedList(pageNumber, pageSize));
            }

            return View("NoAccess");
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
            ticketModel.TicketType = new SelectList(db.TicketType.ToList(), "Id", "Name");
            ticketModel.ProjectId = projectId;
            return View(ticketModel);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Submitter")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectId,Title,Description,FileBase,FileDescription,TicketTypeId")] Ticket ticket, List<TicketAttachmentViewModel> ticketAttachments)
        { 

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
                var dateTimeNow = DateTime.Now;
                if (ticketAttachments != null)
                {
                    foreach (var attach in ticketAttachments)
                    {
                        if (attach.FileBase != null)
                        {

                            var attachmentDB = new TicketAttachment();
                            attachmentDB.AuthorId = User.Identity.GetUserId();
                            attachmentDB.Created = dateTimeNow;
                            attachmentDB.Description = attach.FileDescription;
                            attachmentDB.TicketId = ticket.Id;
                            var hash = attach.GetHashCode();
                            attachmentDB.Name = SlugConverter.URLFriendly(Path.GetFileNameWithoutExtension(attach.FileBase.FileName)) + "-" + hash.ToString() + Path.GetExtension(attach.FileBase.FileName);
                            var content = Server.MapPath("~/uploads/tickets/");
                            var path = Path.Combine(content, Convert.ToString(ticket.Id));
                            Directory.CreateDirectory(path);

                            attach.FileBase.SaveAs(Path.Combine(Server.MapPath("~/uploads/tickets/" + ticket.Id + "/"), attachmentDB.Name));
                            attachmentDB.FilePath = "/uploads/tickets/" + ticket.Id + "/" + attachmentDB.Name;
                            db.TicketAttachments.Add(attachmentDB);
                        }
                        else
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                    }
                }

                ticket.Created = dateTimeNow;
                ticket.AuthorId = User.Identity.GetUserId();
                ticket.TicketTypeId = ticket.TicketTypeId;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index", "Projects");
            }

            var modelTicket = new CreateTicketListModel();
            modelTicket.ProjectId = ticket.ProjectId;
            return View(modelTicket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Project Manager,Developer")]
        [CheckTicketOwnFilter]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            var userDB = UserHelper.GetUserById(User.Identity.GetUserId());
            //if (
            //    (userDB.ProjectsManage.Any(p => p.Id == ticket.ProjectId) && User.IsInRole("Project Manager"))
            //    || ((ticket.DeveloperId == userDB.Id) && User.IsInRole("Developer"))
            //    )
            //{
                if (ticket.TicketPriorityId != null)
                {

                    ViewBag.TicketPriorityId = new SelectList(db.TicketPriority.ToList(), "Id", "Name", ticket.TicketPriorityId);
                }
                else
                {
                    ViewBag.TicketPriorityId = new SelectList(db.TicketPriority.ToList(), "Id", "Name");
                }

                if (ticket.TicketStatusId != null)
                {

                    ViewBag.TicketStatusId = new SelectList(db.TicketStatus.ToList(), "Id", "Name", ticket.TicketStatusId);
                }
                else
                {
                    ViewBag.TicketStatusId = new SelectList(db.TicketStatus.ToList(), "Id", "Name");
                }

                if (ticket.TicketTypeId != null)
                {

                    ViewBag.TicketTypeId = new SelectList(db.TicketType.ToList(), "Id", "Name", ticket.TicketTypeId);
                }
                else
                {
                    ViewBag.TicketTypeId = new SelectList(db.TicketType.ToList(), "Id", "Name");
                }

            //    return View(ticket);
            //}
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckTicketOwnFilter]
        [Authorize(Roles = "Project Manager,Developer")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,TicketTypeId,TicketStatusId,TicketPriorityId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var dateTimeNow = DateTime.Now;
                var ticketDB = db.Tickets.FirstOrDefault(t => t.Id == ticket.Id);
                var userDB = UserHelper.GetUserById(User.Identity.GetUserId());

                //if (
                //    (userDB.ProjectsManage.Any(p => p.Id == ticketDB.ProjectId) && User.IsInRole("Project Manager"))
                //    || ((ticketDB.DeveloperId == userDB.Id) && User.IsInRole("Developer"))
                //    )
                //{

                    var changes = new List<TicketHistory>();

                    ticketDB.Title = ticket.Title;
                    ticketDB.Description = ticket.Description;
                    ticketDB.Updated = dateTimeNow;
                    ticketDB.TicketTypeId = ticket.TicketTypeId;
                    ticketDB.TicketStatusId = ticket.TicketStatusId;
                    ticketDB.TicketPriorityId = ticket.TicketPriorityId;

                    var originalValues = db.Entry(ticketDB).OriginalValues;
                    var currentValues = db.Entry(ticketDB).CurrentValues;

                    foreach (var property in originalValues.PropertyNames)
                    {
                        var originalValue = originalValues[property]?.ToString();
                        var currentValue = currentValues[property]?.ToString();

                        if (originalValue != currentValue)
                        {
                            var history = new TicketHistory();
                            history.Changed = dateTimeNow;
                            //history.NewValue = GetValueFromKey(property, currentValue);
                            //history.OldValue = GetValueFromKey(property, originalValue);
                            history.NewValue = currentValue;
                            history.OldValue = originalValue;

                            history.Property = property;
                            history.TicketId = ticketDB.Id;
                            history.UserId = User.Identity.GetUserId();
                            changes.Add(history);
                        }
                    }

                    db.TicketHistory.AddRange(changes);

                    db.SaveChanges();
                    var devDB = ticketDB.Developer;

                    if (devDB != null)
                    {

                        var newMail = new MailMessage(userDB.Email, devDB.Email);
                        newMail.Subject = $"Ticket {ticket.Title} has changed";
                        newMail.Body = $"<h3>This is email from {userDB.DisplayName}. <p>Ticket attach to you have changed.<p/>";
                        newMail.IsBodyHtml = true;

                        await PersonalEmail.SendAsync(newMail);
                    }

                    
                //}
            }
            return RedirectToAction("Index", "Projects");
        }


        private string GetValueFromKey(string propertyName, string key)
        {
            if (propertyName == "TicketTypeId")
            {
                //return db.TicketTypes.Find(Convert.ToInt32(key)).Name;
            }
            return key;
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
        [Authorize(Roles = "Project Manager")]
        public ActionResult AssignDeveloper(int id)
        {

            var ticket = db.Tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userDB = UserHelper.GetUserById(User.Identity.GetUserId());

            if (userDB.ProjectsManage.Any(p => p.Id == ticket.ProjectId) && User.IsInRole("Project Manager"))
            {
                var viewModel = new AssignTicketToDeveloperModel();
                viewModel.TicketId = id;
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
            return View("NoAccess");
        }


        //POST: Assgin developer to ticjet
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public async Task<ActionResult> AssignDeveloper([Bind(Include = "TicketId,SelectedDevId,ProjectId")] AssignTicketToDeveloperModel model)
        {
            var userDB = UserHelper.GetUserById(User.Identity.GetUserId());

            if (userDB.ProjectsManage.Any(p => p.Id == model.ProjectId) && User.IsInRole("Project Manager"))
            {
                var ticket = db.Tickets.Find(model.TicketId);
                if (ticket == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ticket.DeveloperId = model.SelectedDevId;
                db.SaveChanges();


                var devDB = db.Users.FirstOrDefault(u => u.Id == model.SelectedDevId);

                var newMail = new MailMessage(userDB.Email, devDB.Email);
                newMail.Subject = "New ticket was attached to you.";
                newMail.Body = $"<h3>This is email from {userDB.DisplayName}. <p>New ticket was attached to you.<p/>";
                newMail.IsBodyHtml = true;

                await PersonalEmail.SendAsync(newMail);


                return RedirectToAction("Details", "Projects", new { id = model.ProjectId });
            }
            return View("NoAccess");
        }

        //public void SendTest()

        //{

        //    NotificationHub.SendMessage("demoDEV@demo.com", "Test project", "ticket #4", 6);
        //    //return RedirectToAction("Index", "Home");
        //}

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
