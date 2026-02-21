using backend.Models;

namespace backend.Interfaces;

public interface ILog
{
  List<LogDTO> GetAll();
  LogDTO Add(string DroneID, string Result, DateTime RepairTime, DateTime LogTime);
}