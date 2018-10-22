namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using BugTracker.Models.TicketAddonsModels;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            //AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }



            ApplicationUser adminUser = new ApplicationUser();
            ApplicationUser demoDEV = new ApplicationUser();
            ApplicationUser demoSUB = new ApplicationUser();
            ApplicationUser demoPM = new ApplicationUser();
            // seeding admin
            if (!context.Users.Any(item => item.UserName == "admin@admin.com"))
            {
                adminUser.UserName = "admin@admin.com";
                adminUser.Email = "admin@admin.com";
                adminUser.LastName = "Admin";
                adminUser.FirstName = "Zerg";
                adminUser.DisplayName = "z3rg";


                userManager.Create(adminUser, "admin@admin.com");
            }
            else
            {
                adminUser = context.Users.FirstOrDefault(item => item.UserName == "admin@admin.com");
            }

            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }


            // seeding demo project manager
            if (!context.Users.Any(item => item.UserName == "demoPM@demo.com"))
            {
                demoPM.UserName = "demoPM@demo.com";
                demoPM.Email = "demoPM@demo.com";
                demoPM.LastName = "John";
                demoPM.FirstName = "Doe";
                demoPM.DisplayName = "Project Manager";


                userManager.Create(demoPM, "demoPM@demo.com");
            }
            else
            {
                demoPM = context.Users.FirstOrDefault(item => item.UserName == "demoPM@demo.com");
            }

            if (!userManager.IsInRole(demoPM.Id, "Project Manager"))
            {
                userManager.AddToRole(demoPM.Id, "Project Manager");
            }

            // seeding demo developer
            if (!context.Users.Any(item => item.UserName == "demoDEV@demo.com"))
            {
                demoDEV.UserName = "demoDEV@demo.com";
                demoDEV.Email = "demoDEV@demo.com";
                demoDEV.LastName = "John";
                demoDEV.FirstName = "Doe";
                demoDEV.DisplayName = "Developer";


                userManager.Create(demoDEV, "demoDEV@demo.com");
            }
            else
            {
                demoDEV = context.Users.FirstOrDefault(item => item.UserName == "demoDEV@demo.com");
            }

            if (!userManager.IsInRole(demoDEV.Id, "Developer"))
            {
                userManager.AddToRole(demoDEV.Id, "Developer");
            }

            // seeding demo submitter
            if (!context.Users.Any(item => item.UserName == "demoSUB@demo.com"))
            {
                demoSUB.UserName = "demoSUB@demo.com";
                demoSUB.Email = "demoSUB@demo.com";
                demoSUB.LastName = "John";
                demoSUB.FirstName = "Doe";
                demoSUB.DisplayName = "Submitter";


                userManager.Create(demoSUB, "demoSUB@demo.com");
            }
            else
            {
                demoSUB = context.Users.FirstOrDefault(item => item.UserName == "demoSUB@demo.com");
            }

            if (!userManager.IsInRole(demoSUB.Id, "Submitter"))
            {
                userManager.AddToRole(demoSUB.Id, "Submitter");
            }


            //seeding types table
            if (!context.ProjectTypes.Any())
            {
                context.ProjectTypes.AddOrUpdate(item => item.Id,
                    new ProjectType() { Type = "Angular", HexColor = "#c30e2e" },
                    new ProjectType() { Type = "Vuejs", HexColor = "#4dba87" },
                    new ProjectType() { Type = "React", HexColor = "#61dafb" },
                    new ProjectType() { Type = "ASP.NET", HexColor = "#60399e" },
                    new ProjectType() { Type = "Nodejs", HexColor = "#3c823b" }
                    );
            }


            //seeding ticket status table
            if (!context.TicketStatus.Any())
            {
                context.TicketStatus.AddOrUpdate(item => item.Id,
                   new TicketStatus() { Name = "Opened", HexColor = "#85C1E9" },
                   new TicketStatus() { Name = "In develop", HexColor = "#E8DAEF" },
                   new TicketStatus() { Name = "Closed", HexColor = "#27AE60" }
                    );
            }

            //seeding ticket type table
            if (!context.TicketType.Any())
            {
                context.TicketType.AddOrUpdate(item => item.Id,
                   new TicketType() { Name = "Bug", HexColor = "#f5cba7" },
                   new TicketType() { Name = "Error", HexColor = "#c39bd3" },
                   new TicketType() { Name = "UI", HexColor = "#A2D9CE" }
                    );
            }

            //seeding ticket priority table
            if (!context.TicketPriority.Any())
            {
                context.TicketPriority.AddOrUpdate(item => item.Id,
                   new TicketPriority() { Name = "High", HexColor = "#F39C12" },
                   new TicketPriority() { Name = "Low" , HexColor = "#2874A6" }
                    );
            }

        }
    }
}
