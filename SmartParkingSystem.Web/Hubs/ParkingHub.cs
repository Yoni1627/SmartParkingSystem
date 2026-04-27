using Microsoft.AspNetCore.SignalR;

namespace SmartParkingSystem.Web.Hubs
{
    public class ParkingHub : Hub
    {
        private static int _viewingCount = 0;

        public override async Task OnConnectedAsync()
        {
            Interlocked.Increment(ref _viewingCount);
            await Clients.All.SendAsync("UpdateViewingCount", _viewingCount);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Interlocked.Decrement(ref _viewingCount);
            await Clients.All.SendAsync("UpdateViewingCount", _viewingCount);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task BroadcastSlotStatusChange(int slotId, string status)
        {
            await Clients.All.SendAsync("ReceiveSlotStatusUpdate", slotId, status);
        }
    }
}
