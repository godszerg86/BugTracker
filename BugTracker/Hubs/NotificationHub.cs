using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace BugTracker.Hubs
{
    [HubName("notificationHub")]
    public class NotificationHub : Hub
    {
        

        public void SendMessage(string userId,string projName, string ticketName, int projectId)
        {
            //var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();

            //hubContext.Clients.User(userId).sendNotificationToDeveloper(projName, ticketName, projectId);
        
            Clients.User(userId).sendNotificationToDeveloper(projName, ticketName, projectId);
        }
    }




    //public class CustomUserIdProvider : IUserIdProvider
    //{
    //    public string GetUserId(IRequest request)
    //    {

    //        var userId = request.User.Identity.GetUserName();
    //        return userId.ToString();
    //    }
    //}
}