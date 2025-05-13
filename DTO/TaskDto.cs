namespace Backend_RC.DTO;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Icon { get; set; }
    public string TaskPointStart { get; set; }
    public string TaskPointEnd { get; set; }
    public string Image { get; set; }
    public string Description { get; set; }
    public string TaskDescription { get; set; }
    public string Reward { get; set; }
    public string TaskCityType { get; set; }
    public string TaskCityPlaceInfo { get; set; }
    public double[] StartCoordinates { get; set; }
    public double[] EndCoordinates { get; set; }
}


