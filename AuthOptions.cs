

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspNetWebServer;

public class AuthOptions
{
    public const string ISSUER = "InfoWatcherClientAspNetServer"; // издатель токена
    public const string AUDIENCE = "InfoWatcherClient"; // потребитель токена
    const string KEY = "LgArJYNxAYexCJlbkGNVBjooSwik8Y4D0eRq80PhWmGGl5FD1GqEVL";   // ключ для шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => 
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}