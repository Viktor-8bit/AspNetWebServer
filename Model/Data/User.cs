namespace AspNetWebServer.Model.Data
{
    public class User
    {
        public int Id { get; set; }
        public string HashPassword { get; set; }
        public string Login { get; set; }
        public Pc PC { get; set; }
    }
}
