using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Webapi.Middlewares
{
    public class JWTMiddleware
    {
        private readonly RequestDelegate _req;
        
        private readonly IConfiguration _config;
        public JWTMiddleware(RequestDelegate req, IConfiguration config)
        {
            _req = req;            
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await attachAccount(context, token);

            await _req(context);
        }

        private async Task attachAccount(HttpContext context, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("JWTConfig:Secret"));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _config.GetValue<string>("JWTConfig:issuer"),
                    ValidAudience = _config.GetValue<string>("JWTConfig:audienceid")
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                string Id = Convert.ToString(jwtToken.Claims.First(x => x.Type == "Session").Value);
                context.Items["session"] = Id;
            }
            catch
            {

                // return null if validation fails
                context.Items["session"] = null;
            }
            
        }
     }
}
