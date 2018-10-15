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
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db { get; set; }
        private UserHelper UserHelper { get; set; }

        public ProjectsController()
        {
            db = new ApplicationDbContext();
            UserHelper = new UserHelper(db);

        }


        // GET: Projects
        public ActionResult Index(string query)
        {

            if ((User.Identity.IsAuthenticated && User.IsInRole("Submitter")) || (User.Identity.IsAuthenticated && User.IsInRole("Developer")))
            {
                var userDB = UserHelper.GetUserById(User.Identity.GetUserId());
                if (string.IsNullOrEmpty(query) || string.IsNullOrWhiteSpace(query))
                {
                    return View(userDB.ProjectsManage.ToList());

                }
                else
                {
                    var tempList = userDB.ProjectsManage.Where(p => p.Name.ToLower().Contains(query.ToLower()) || p.Description.ToLower().Contains(query.ToLower())).ToList();
                    return View(tempList);
                }
            }

            //if (User.Identity.IsAuthenticated && User.IsInRole("Developer"))
            //{
            //    var userDB = UserHelper.GetUserById(User.Identity.GetUserId());
            //    return View(userDB.ProjectsManage.ToList());
            //}

            if ((User.Identity.IsAuthenticated && User.IsInRole("Project Manager")) || (User.Identity.IsAuthenticated && User.IsInRole("Admin")))
            {
                if (string.IsNullOrEmpty(query) || string.IsNullOrWhiteSpace(query))
                {
                    return View(db.Projects.ToList());
                }
                else
                {
                    return View(db.Projects.Where(p => p.Name.ToLower().Contains(query.ToLower()) || p.Description.ToLower().Contains(query.ToLower())).ToList());
                }
            }

            //if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            //{
            //    return View(db.Projects.ToList());
            //}

            //Demo functionalitiy

            //if (User.Identity.IsAuthenticated && User.IsInRole("Demo User"))
            //{
            //    return View(db.Projects.ToList());
            //}

            return View();
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            ViewBag.ProjectTypeId = new SelectList(db.ProjectTypes, "Id", "Type");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,ProjectTypeId,Created")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.AuthorId = User.Identity.GetUserId();
                project.Created = DateTime.Now;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectTypeId = new SelectList(db.ProjectTypes, "Id", "Type");
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectTypeId,Name,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                var projectDB = db.Projects.FirstOrDefault(item => item.Id == project.Id);
                projectDB.ProjectTypeId = project.ProjectTypeId;
                projectDB.Name = project.Name;
                projectDB.Description = project.Description;
                projectDB.Updated = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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
