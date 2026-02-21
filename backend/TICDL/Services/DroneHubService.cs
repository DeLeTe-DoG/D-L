using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

public class DroneHubService : Hub
{

  public async Task SendMission(object missionData)
  {
    var json = JsonSerializer.Serialize(missionData);
    await Clients.All.SendAsync("RecieveTask", json);
  }

  public override async Task OnConnectedAsync()
  {
    Console.WriteLine($"[HUB] Дрон подключился: {Context.ConnectionId}");
    await base.OnConnectedAsync();
  }

}