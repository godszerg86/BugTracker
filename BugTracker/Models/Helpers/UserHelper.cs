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
        private UserManager<ApplicationUser> UserManager { get; set; }
        private RoleManager<IdentityRole> RoleManager { get; set; }


        public UserHelper(ApplicationDbContext db)
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


        public List<Project> GetAllProjectsAssignedToUser(string id)
        {
            return this.GetUserById(id).ProjectsManage.ToList();
        }


        internal void RemoveFromRoles(string id, string[] roles)
        {
            UserManager.RemoveFromRoles(id, roles);
        }


        internal void AddToRole(string id, string selectedRole)
        {
            UserManager.AddToRole(id, selectedRole);
        }


        internal List<IdentityRole> GetAllAppRoles()
        {
            return RoleManager.Roles.ToList();
        }

        public ICollection<ApplicationUser> GetUsersInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            var List = UserManager.Users.ToList();
            foreach (var user in List)
            {
                if (IsUserInRole(user.Id, roleName))
                {

                    resultList.Add(user);
                }
            }
            return resultList;
        }

        internal ApplicationUser GetDemoUser(string role)
        {
            ApplicationUser returnUser = null;
            if (role == "Project Manager")
            {
                returnUser = UserManager.FindByEmail("demoPM@demo.com");
            }
            if (role == "Developer")
            {
                returnUser = UserManager.FindByEmail("demoDEV@demo.com");
            }
            if (role == "Submitter")
            {
                returnUser = UserManager.FindByEmail("demoSUB@demo.com");
            }

            return returnUser;
        }

        public ICollection<ApplicationUser> GetUsersNotInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            var List = UserManager.Users.ToList();
            foreach (var user in List)
            {
                if (!IsUserInRole(user.Id, roleName))
                    resultList.Add(user);
            }
            return resultList;
        }


        public bool IsUserInRole(string userId, string roleName)
        {
            return UserManager.IsInRole(userId, roleName);
        }
    }
}