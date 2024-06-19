using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class PriceHub : Hub
{
    // This method allows a client to join a group for a specific financial instrument symbol.
    public async Task Subscribe(string symbol)
    {
        // Add the client's connection to a group for the specified symbol.
        // This allows for efficient broadcasting of price updates to all clients subscribed to this symbol.
        await Groups.AddToGroupAsync(Context.ConnectionId, symbol);
    }

    // This method allows a client to leave a group for a specific financial instrument symbol.
    public async Task Unsubscribe(string symbol)
    {
        // Remove the client's connection from the group for the specified symbol.
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, symbol);
    }

    // This method broadcasts price updates to all clients subscribed to a specific symbol group.
    public async Task BroadcastPriceUpdate(string symbol, decimal price)
    {
        // SignalR efficiently handles broadcasting messages to groups of clients.
        // By grouping clients based on the symbol, we ensure that only clients interested in the symbol
        // receive the updates, reducing unnecessary network traffic and improving performance.
        await Clients.Group(symbol).SendAsync("ReceivePriceUpdate", symbol, price);
    }
}
