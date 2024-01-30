namespace AspNetWebServer.Model.Data.ProcessMonitoring;

public class MountedProcess
{
    public int Id { get; set; }
    public int MonutedIndex { get; set; }
    public string Name { get; set; }
    public int ProcessId { get; set; }
    public Pc PcSender { get; set; }
    public DateTime Date { get; set; }

}