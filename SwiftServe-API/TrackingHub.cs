using Microsoft.AspNetCore.SignalR;

namespace SwiftServe_API
{
    public class TrackingHub : Hub
    {
        public async Task SendLocation(int orderId, double lat, double lng)
        {
            await Clients.Group(orderId.ToString())
                .SendAsync("ReceiveLocation", lat, lng);
        }

        public async Task JoinOrderGroup(int orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, orderId.ToString());
        }
    }
}
