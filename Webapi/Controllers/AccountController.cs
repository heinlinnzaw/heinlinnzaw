using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Services;
using Webapi.Models;
using Webapi.Helpers;
using Microsoft.Extensions.Configuration;
using Webapi.Entities;

namespace Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IConfiguration _config;
        public AccountController(ICustomerRepository customerRepository, ITokenRepository tokenRepository, IConfiguration config)
        {            
            _customerRepository = customerRepository;
            _config = config;
            _tokenRepository = tokenRepository;
        }

        [HttpPost("login")]
        public IActionResult login(LoginModel model)
        {
            RespAuthModel resp = new RespAuthModel();
            
            Customer cust = _customerRepository.GetLogin(model.username, model.pwd);
            string tId = Guid.NewGuid().ToString();
            if (cust != null)
            {  
             
                resp.refresh_token = JWTGenerator.RandomString(50);
                while (_tokenRepository.QueryByrefreshId(resp.refresh_token) != null)
                {
                    resp.refresh_token = JWTGenerator.RandomString(50);
                }

                resp.access_token = JWTGenerator.GenerateJwtToken(cust.Id,
                                _config.GetValue<string>("JWTConfig:Secret"),
                                _config.GetValue<string>("JWTConfig:issuer"),
                                _config.GetValue<string>("JWTConfig:audienceid"), tId);
                resp.expires = DateTime.Now.AddDays(7);
                resp.username = model.username;
                resp.user_id = cust.Id;                
                resp.phone = cust.phone;

                _tokenRepository.CreateToken(new TokenEntity
                {
                    Token_Id = tId,
                    Customer_Id = cust.Id,
                    Token = resp.access_token,
                    Created_On = DateTime.Now,
                    Is_Active = true,
                    Expires = resp.expires,
                    Refresh_Token = resp.refresh_token
                });
                _tokenRepository.RevokeToken(new TokenEntity { Token_Id = tId, Customer_Id = cust.Id, Revoked_On = DateTime.Now, Is_Active = false });
                _customerRepository.UpdateLastLogin(cust);
                return Ok(resp);
            }
            else
                return BadRequest(new { Code = "1000", Message = "Invalid LoginId or Pwd." });
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenModel model)
        {
            RespRefreshToken resp = new RespRefreshToken();            
            TokenEntity tEntity = await Task.Run(()=> _tokenRepository.QueryByrefreshId(model.refresh_token));
            if(tEntity != null)
            {
                if(tEntity.Expires > DateTime.Now)
                {
                    resp.refresh_token = JWTGenerator.RandomString(50);
                    while (_tokenRepository.QueryByrefreshId(resp.refresh_token) != null)
                    {
                        resp.refresh_token = JWTGenerator.RandomString(50);
                    }
                    
                    resp.access_token = JWTGenerator.GenerateJwtToken(model.id, tEntity.Token_Id,
                               _config.GetValue<string>("JWTConfig:Secret"),
                               _config.GetValue<string>("JWTConfig:issuer"),
                               _config.GetValue<string>("JWTConfig:audienceid"));

                    _tokenRepository.UpdateToken(new TokenEntity
                    {
                        Token_Id = tEntity.Token_Id,
                        Token = resp.access_token,
                        Refresh_Token = resp.refresh_token
                    });

                    return Ok(resp);
                }
                else
                {
                    return BadRequest(new { Code = "1002", Message = "Refresh token had expired" });
                }
            }
            else
            {
                return BadRequest(new { Code = "1001", Message = "Invalid refresh token"});
            }
        }
    }
}
