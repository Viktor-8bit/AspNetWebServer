namespace AspNetWebServer.Model.Data;

public class infoSecuritySpecialist
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string HashPassword { get; set; }
    public string Salt { get; set; }
}