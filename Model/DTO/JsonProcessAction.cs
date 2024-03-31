using System.Runtime.InteropServices.JavaScript;

namespace AspNetWebServer.Model.DTO;

public class JsonProcessAction
{
    public char Action { get; set; }
    public string Name { get; set; }
    public int Id { get; set; }
    public DateTime TimeStarted { get; set; }
}