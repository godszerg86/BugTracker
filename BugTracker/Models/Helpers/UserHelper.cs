using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Helpers
{
    public class UserHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> UserManager { get; set; }
        private RoleManager<IdentityRole> RoleManager { get; set; }


        public UserHelper()
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }

        public List<string> GetRoles(string id)
        {
            return UserManager.GetRoles(id).ToList();
        }

        public ApplicationUser GetUserById(string id)
        {
            return UserManager.FindById(id);
        }


        public List<Project> GetAllProjectsAssignedToUser (string id)
        {
           return this.GetUserById(id).ProjectsManage.ToList();
        }

        internal void RemoveFromRoles(string id, string[] roles)
        {
            UserManager.RemoveFromRoles(id,roles);
        }

        internal void AddToRole(string id, string selectedRole)
        {
            UserManager.AddToRole(id, selectedRole);        }

        internal List<IdentityRole> GetAllAppRoles()
        {
           return RoleManager.Roles.ToList();
        }
    }
}