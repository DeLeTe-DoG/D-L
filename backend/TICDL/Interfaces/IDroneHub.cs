namespace backend.Interfaces;

public interface IDroneHub
{
  public Task SendMission(object missionData);
}