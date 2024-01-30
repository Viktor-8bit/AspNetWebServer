using System.Data.Entity;

namespace AspNetWebServer.Model.Data;

public class Pc
{    

    public int Id { get; set; }
    public string hostname { get; set; }
    public bool Online { get; set; }
}