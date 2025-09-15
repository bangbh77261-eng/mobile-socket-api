// Services/AuthenticatedWebSocketManager.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthenticatedWebSocketManager : WebSocketManager
{
    private readonly IConfiguration _configuration;

    public AuthenticatedWebSocketManager(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> ValidateTokenAsync(HttpContext context)
    {
        var token = context.Request.Query["token"].ToString();

        if (string.IsNullOrEmpty(token))
            return false;

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == "userId").Value;

            // Add user info to context
            context.Items["UserId"] = userId;
            return true;
        }
        catch
        {
            return false;
        }
    }
}