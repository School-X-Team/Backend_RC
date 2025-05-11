namespace Backend_RC.Models;
public class Routee
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Time { get; set; }
    public string Image { get; set; }
    public string Description { get; set; }
    public double[] StartCoordinates { get; set; }
    public double[] EndCoordinates { get; set; }
}
