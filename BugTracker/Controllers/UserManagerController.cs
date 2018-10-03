using BugTracker.Models;
using BugTracker.Models.classes;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class UserManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager { get; set; }
        public ICollection<UserListModel> userList { get; set; }
        public UserManagerController()
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: UserManager
        public ActionResult Index()
        {

            userList = new HashSet<UserListModel>();
            ICollection<ApplicationUser> usersDB = db.Users.ToList();

            foreach (var user in usersDB)
            {
                UserListModel userModel = new UserListModel();
                userModel.DisplayName = user.DisplayName;
                userModel.LastName = user.LastName;
                userModel.FirstName = user.FirstName;
                userModel.Id = user.Id;
                userModel.ProjectsCreated = user.ProjectsCreated.ToList();
                userModel.ProjectAssigned = user.ProjectsManage.ToList();
                userModel.Roles = userManager.GetRoles(user.Id);
                userList.Add(userModel);
            }

            return View(userList);
        }

        //GET: ManageUser
        public ActionResult ManageUserAssignedProjects(string id)
        {
            var userDB = userManager.FindById(id);
            UserListModel userModel = new UserListModel();
            userModel.DisplayName = userDB.DisplayName;
            userModel.ProjectAssigned = userDB.ProjectsManage.ToList();

            var projectsDB = db.Projects.ToList();

            foreach (var item in userModel.ProjectAssigned)
            {
                projectsDB.Remove(item);

            }

            userModel.ProjectsNotAssigned = projectsDB;

            return View(userModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageUserAssignedProjects(int[] assign, string id)
        {
            var userDB = userManager.FindById(id);
            userDB.ProjectsManage.Clear();
            if (assign != null)
            {
                var projectsDB = db.Projects.ToList();
                foreach (var item in assign)
                {
                    var arrayProject = projectsDB.FirstOrDefault(proj => proj.Id == item);
                    userDB.ProjectsManage.Add(arrayProject);
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult ManageUserRoles (string id)
        {
            return View();
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