using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using signalrCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace signalrApi.Hubs
{
    [Authorize]
    public class MainHub : Hub
    {
        public string idTenant;
        public string logServer;

        private readonly IHttpClientFactory _clientFactory;

        public MainHub(IHttpClientFactory clientFactory)
        {
            logServer = Environment.GetEnvironmentVariable("LOG_URL");
            _clientFactory = clientFactory;
        }

        public static List<userConnected> _usersConnected = new List<userConnected>();

        public void connectUser(userConnected connectUserData)
        {
            var userId = Context.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var LastName = Context.User.Claims.FirstOrDefault(x => x.Type == "LastName")?.Value;

            if (!string.IsNullOrEmpty(connectUserData.appId) && !string.IsNullOrEmpty(userId))
            {
                var _userConnected = new userConnected()
                {
                    connectionId = Context.ConnectionId,
                    userId = connectUserData.userId,
                    appId = connectUserData.appId,
                    deviceType = connectUserData.deviceType,
                    deviceDesc = connectUserData.deviceDesc,
                    Name = Context.User.Identity.Name,
                    LastName = LastName,
                    companyId = connectUserData.companyId
                };

                if (!_usersConnected.Any(x => x.connectionId == Context.ConnectionId))
                {
                    _usersConnected.Add(_userConnected);
                }

                insertLogData(_userConnected, "Connected");
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userConnected = _usersConnected.FirstOrDefault(x => x.connectionId == Context.ConnectionId);
            if (userConnected != null)
            {
                insertLogData(userConnected, "Disconnected");
                _usersConnected.Remove(userConnected);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public List<userConnected> getUserConnecteds(string companyId) 
        {
            return _usersConnected.Where(x => x.companyId == companyId).ToList();
        }

        public void updateUrlUser(string url) 
        {
            var userConnected = _usersConnected.FirstOrDefault(x => x.connectionId == Context.ConnectionId);
            if (userConnected != null)
            {
                userConnected.url = url;
                insertLogData(userConnected, "Navigation");
            }
        }

        private async void insertLogData(userConnected userConnected, string type)
        {
            var logChange = new LogChange
            {
                InfoForm = type,
                NewValue = userConnected.url
            };

            var request = new HttpRequestMessage(HttpMethod.Post, logServer + "/api/LogChanges");
            request.Headers.Add("x-tenant-id", userConnected.companyId);
            request.Headers.Add("Authorization", "Bearer " + Startup.token);
            request.Content = new StringContent(JsonConvert.SerializeObject(logChange), Encoding.UTF8, "application/json");
            var client = _clientFactory.CreateClient("logApi");
            var response = await client.SendAsync(request);

            using var responseStream = await response.Content.ReadAsStreamAsync();

        }
    }
}
