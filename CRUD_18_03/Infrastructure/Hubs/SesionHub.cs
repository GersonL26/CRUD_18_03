using Microsoft.AspNetCore.SignalR;

namespace CRUD_18_03.Infrastructure.Hubs;

public class SesionHub : Hub
{
    public async Task UnirseASesion(string sesionId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"sesion-{sesionId}");

    public async Task SalirDeSesion(string sesionId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"sesion-{sesionId}");

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
