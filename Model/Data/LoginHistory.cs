namespace AspNetWebServer.Model.Data;

public class LoginHistory
{
    public int Id { get; set; }

    public infoSecuritySpecialist User { get; set; }

    public DateTime LoginTime { get; set; }

    public string IpAddress { get; set; }

    public string UserAgent { get; set; }
}