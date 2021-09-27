using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using signalrCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalrApi.Hubs
{
    [Authorize]
    public class MainHub : Hub
    {
        public static List<userConnected> _usersConnected = new List<userConnected>();

        public void connectUser(userConnected connectUserData)
        {
            if (!string.IsNullOrEmpty(connectUserData.appId) && !string.IsNullOrEmpty(connectUserData.userId))
            {
                var _userConnected = new userConnected()
                {
                    connectionId = Context.ConnectionId,
                    userId = connectUserData.userId,
                    appId = connectUserData.appId,
                    deviceType = connectUserData.deviceType,
                    deviceDesc = connectUserData.deviceDesc,
                    Name = connectUserData.Name,
                    LastName = connectUserData.LastName,
                    companyId = connectUserData.companyId
                };

                if (!_usersConnected.Any(x => x.connectionId == Context.ConnectionId))
                {
                    _usersConnected.Add(_userConnected);
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userConnected = _usersConnected.FirstOrDefault(x => x.connectionId == Context.ConnectionId);
            if (userConnected != null)
            {
                _usersConnected.Remove(userConnected);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
