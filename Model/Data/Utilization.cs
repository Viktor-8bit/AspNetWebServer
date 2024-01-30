namespace AspNetWebServer.Model.Data;

public class Utilization
{
    public int Id { get; set; }
    public Pc Pc { get; set; }
    public float CPU_load { get; set; }
    public float RAM { get; set; }
    public DateTime Date { get; set; }

}