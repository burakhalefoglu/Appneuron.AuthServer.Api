using Business.MessageBrokers.SignalR.Models;
using Core.Extensions;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business.MessageBrokers.SignalR
{
    public class TestHub:Hub
    {
        public static List<CustomerModel> userList = new List<CustomerModel>();

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
           
            var query = httpContext.Request.Query;
            var userId = query.GetQueryParameterValue<int>("UserId");
            var connectionId = Context.ConnectionId;

            var result= userList.Find(u => u.UserId == userId);
            if (result == null)
            {
                userList.Add(new CustomerModel
                {
                    UserId = userId,
                    ConnectionId = connectionId
                });
                return base.OnConnectedAsync();
            }

            result.ConnectionId = connectionId;
            return base.OnConnectedAsync();
        }
    }
}
