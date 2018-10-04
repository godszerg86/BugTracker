using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models.classes
{
    public class UserListModel
    {
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Id { get; set; }
        public ICollection<Project> ProjectsCreated { get; set; }
        public ICollection<Project> ProjectsNotAssigned { get; set; }
        public ICollection<Project> ProjectAssigned { get; set; }
        public ICollection<string> Roles { get; set; }
        public MultiSelectList RolesList { get; set; }
        public string[] SelectedRoles { get; set; }

    }
}