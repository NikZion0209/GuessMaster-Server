using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

public class JwtRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;

    public JwtRefreshMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Example: refresh if less than 10 minutes left
            var exp = jwtToken.ValidTo;
            if (exp < DateTime.UtcNow.AddMinutes(10))
            {
                var claims = jwtToken.Claims.ToList();

                claims.Add(new Claim("sessionId", "newSessionId"));

                var key = _config["Jwt:Key"];
                var issuer = _config["Jwt:Issuer"];
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var newToken = new JwtSecurityToken(
                    issuer: issuer,
                    audience: null,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: credentials
                );

                var refreshedToken = handler.WriteToken(newToken);
                context.Response.Headers["X-Refreshed-Token"] = refreshedToken;
            }
        }

        await _next(context);
    }
}