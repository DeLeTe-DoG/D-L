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
      Console.WriteLine($"\n[SIGNALR] ===> НОВОЕ ПОДКЛЮЧЕНИЕ: {Context.ConnectionId}");
      Console.WriteLine($"[SIGNALR] User: {Context.UserIdentifier}");
      await base.OnConnectedAsync();
  }
  public override async Task OnDisconnectedAsync(Exception? exception)
  {
      Console.WriteLine($"\n[SIGNALR] <=== ОТКЛЮЧЕНИЕ: {Context.ConnectionId}");
      if (exception != null)
      {
          Console.WriteLine($"[SIGNALR] ПРИЧИНА: {exception.Message}");
      }
      await base.OnDisconnectedAsync(exception);
  }
}

