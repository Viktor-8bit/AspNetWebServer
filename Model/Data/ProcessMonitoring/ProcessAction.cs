namespace AspNetWebServer.Model.Data.ProcessMonitoring;

public enum ProcessActions
{
    Opened,
    Closed
}

public class ProcessAction
{
    public int Id { get; set; }
    public ProcessActions Action { get; set; }
    public string Name { get; set; }
    public int ProcessId { get; set; }
    public MountedProcess MountedProcess { get; set; }
    public Pc PcSender { get; set; }
    public DateTime Date { get; set; }
    
}