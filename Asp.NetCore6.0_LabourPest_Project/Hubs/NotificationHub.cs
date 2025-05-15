using Microsoft.AspNetCore.SignalR;
using Asp.NetCore6._0_LabourPest_Project.Hubs;
using System.Security.Claims;


namespace Asp.NetCore6._0_LabourPest_Project.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // Her connection’ı kendi kullanıcı kimliğine eşit bir gruba ekle
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }
    }
}
