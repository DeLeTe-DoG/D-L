using Microsoft.AspNetCore.SignalR;
using backend.Service;
using backend.Interfaces;
using backend.Models; // Укажи свой контекст БД

public class LanternMonitorService : BackgroundService
{

    private readonly IHubContext<DroneHubService> _hubContext;
    private readonly IAdmin _adminService;


    public class LanternNode
    {
        public LanternDTO Lantern { get; set; }
        public LanternNode? Left { get; set; }
        public LanternNode? Right { get; set; }

        public LanternNode(LanternDTO lantern)
        {
            Lantern = lantern;
        }
    }

    public static LanternNode? BuildBinaryTree(
      List<LanternDTO> lanterns,
      double centerLat,
      double centerLng)
    {
        if (lanterns == null || lanterns.Count == 0)
            return null;

        LanternNode? root = null;

        foreach (var lantern in lanterns)
        {
            root = Insert(root, lantern);
        }
        return root;

        LanternNode Insert(LanternNode? node, LanternDTO lantern)
        {
            if (node == null)
                return new LanternNode(lantern);
            var current = node;
            double newDist = GetDistance(lantern);
            double currentDist = GetDistance(current.Lantern);

            if (newDist < currentDist)
                current.Left = Insert(current.Left, lantern);
            else
                current.Right = Insert(current.Right, lantern);
            return current;
        }
        double GetDistance(LanternDTO l)
        {
            double dx = l.Coordinates.lat - centerLat;
            double dy = l.Coordinates.lng - centerLng;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    public static List<List<LanternDTO>> GetRoutesToBroken(LanternNode? root)
    {
        var routes = new List<List<LanternDTO>>();
        if (root == null)
            return routes;
        Traverse(root, new List<LanternDTO>());
        return routes;
        void Traverse(LanternNode? node, List<LanternDTO> currentPath)
        {
            if (node == null)
                return;

            currentPath.Add(node.Lantern);

            if (node.Lantern.Status == 1)
            {
                routes.Add(new List<LanternDTO>(currentPath));
            }

            Traverse(node.Left, currentPath);
            Traverse(node.Right, currentPath);

            currentPath.RemoveAt(currentPath.Count - 1);
        }
    }

    public async Task CompleteMission()
    {
        _adminService.isDroneBusy = false;
    }

    public LanternMonitorService(
        IAdmin adminService,
        IHubContext<DroneHubService> hubContext)
    {
        _adminService = adminService;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_adminService.isDroneBusy == false)
            {

                var context = _adminService.shareDB();
                var faultyLanterns = context
                    .Where(l => l.Status == 1)
                    .ToList();
                if (faultyLanterns.Any())
                {
                    Console.WriteLine("[СИСТЕМА] Найдены неисправности!");
                    var allLaneterns = _adminService.GetAllLanterns();
                    var TaskTree = GetRoutesToBroken(BuildBinaryTree(allLaneterns, 0.0, 0.0));
                    await _hubContext.Clients.All.SendAsync("RecieveMission", TaskTree);
                    //_adminService.isDroneBusy = true;
                }
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}