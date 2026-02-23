using Microsoft.AspNetCore.SignalR;
using backend.Interfaces;
using Microsoft.AspNetCore.Cors;
using backend.Models;

namespace backend.Service
{
    public class DroneHubService : Hub
    {
        private readonly IAdmin _adminService;

        public async Task FixLantern(string lanternId)
        {
            var lantern = _adminService.GetAllLanterns().FirstOrDefault(l => l.Id == lanternId);
            if (lantern != null)
            {
                lantern.Status = 0;
                Console.WriteLine($"[СИСТЕМА] Фонарь {lanternId} был исправлен.");
            }
        }

        public async Task CompleteMission()
        {
            _adminService.isDroneBusy = false; // СБРАСЫВАЕМ ФЛАГ
            Console.WriteLine($"[СЕРВЕР] Дрон {Context.GetHttpContext()?.Request.Query["droneId"].ToString()} завершил задание");
        }

        public async Task StartMission()
        {
            _adminService.isDroneBusy = true;
            Console.WriteLine($"[СЕРВЕР] Дрон {Context.GetHttpContext()?.Request.Query["droneId"].ToString()} начал задание");
        }

        public DroneHubService(IAdmin adminService)
        {
            _adminService = adminService;
        }
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var droneId = httpContext?.Request.Query["droneId"].ToString();

            if (!string.IsNullOrEmpty(droneId))
            {
                _adminService.AddDrone("Drone_" + droneId);

                await Clients.All.SendAsync("ReceiveAction", $"Система: Дрон {droneId} подключен к сети.");

                Console.WriteLine($"[Hub] Подключено устройство: {droneId}");
            }

            await base.OnConnectedAsync();
        }

        // public async Task SendTelemetry(string droneId, string data)
        // {
        //     await Clients.All.SendAsync("ReceiveTelemetry", droneId, data);

        //     Console.WriteLine($"[Telemetry] От {droneId}: {data}");
        // }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"[Hub] Устройство отключено: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendComandToDrone(string droneId, string command)
        {
            await Clients.All.SendAsync("RecieveCommand", command);
            Console.WriteLine($"[HUB] Команда {command} отправлена на дрон {droneId}");
        }

        public async Task SendData(object data)
        {
            await Clients.All.SendAsync("RecieveTelemetry", data);
            Console.WriteLine($"[HUB] Карта маршрута отправлена на дронхаб");
        }
    }
}