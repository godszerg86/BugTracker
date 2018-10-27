using BugTracker.Models;
using BugTracker.Models.classes;
using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class UserManagerController : Controller
    {
        private ApplicationDbContext db { get; set; }
        private UserHelper UserHelper { get; set; }
        public ICollection<UserListModel> userList { get; set; }
        public UserManagerController()
        {
            db = new ApplicationDbContext();
            UserHelper = new UserHelper(db);
        }

        // GET: UserManager
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult Index()
        {
            userList = new HashSet<UserListModel>();
            ICollection<ApplicationUser> usersDB = db.Users.ToList();
            //var userHelper = new UserHelper();
            foreach (var user in usersDB)
            {
                if (user.Email != "admin@admin.com")
                {
                    UserListModel userModel = new UserListModel();
                    userModel.DisplayName = user.DisplayName;
                    userModel.LastName = user.LastName;
                    userModel.FirstName = user.FirstName;
                    userModel.Id = user.Id;
                    userModel.ProjectsCreated = user.ProjectsCreated.ToList();
                    userModel.ProjectAssigned = user.ProjectsManage.ToList();
                    userModel.Roles = UserHelper.GetRoles(user.Id);
                    userList.Add(userModel);
                }
            }

            return View(userList);
        }

        //GET: ManageUser
        [Authorize(Roles = "Project Manager")]
        public ActionResult ManageUserAssignedProjects(string id)
        {
            var userDB = UserHelper.GetUserById(id);
            UserListModel userModel = new UserListModel();
            userModel.DisplayName = userDB.DisplayName;
            userModel.ProjectAssigned = UserHelper.GetAllProjectsAssignedToUser(id);

            var projectsDB = db.Projects.ToList();



            foreach (var item in userModel.ProjectAssigned)
            {
                var proj = projectsDB.FirstOrDefault(p => p.Id == item.Id);
                projectsDB.Remove(proj);

            }

            userModel.ProjectsNotAssigned = projectsDB;

            return View(userModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        public ActionResult ManageUserAssignedProjects(int[] assign, string id)
        {

            var userDB = UserHelper.GetUserById(id);
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

        [Authorize(Roles = "Admin")]
        public ActionResult ManageUserRoles(string id)
        {
            var userDB = UserHelper.GetUserById(id);
            UserListModel userModel = new UserListModel();
            userModel.DisplayName = userDB.DisplayName;
            userModel.Id = userDB.Id;
            var appRoles = UserHelper.GetAllAppRoles();
            userModel.Roles = UserHelper.GetRoles(id);


            //var userRolesIds = (from userRole in userDB.Roles
            //                 join role in db.Roles on userRole.RoleId
            //                 equals role.Id
            //                 select role.Id).ToList();



            userModel.RolesList = new SelectList(appRoles, "Name", "Name", userModel.Roles.FirstOrDefault());
            return View(userModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult ManageUserRoles(string selectedRole, string id)
        {
            var userDB = UserHelper.GetUserById(id);
            var roles = UserHelper.GetRoles(id).ToArray();

            UserHelper.RemoveFromRoles(id, roles);
            UserHelper.AddToRole(id, selectedRole);

            //: Refresh authentication cookies so the roles are updated instantly
            //var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            //signInManager.SignIn(userDB, isPersistent: false, rememberBrowser: false);
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