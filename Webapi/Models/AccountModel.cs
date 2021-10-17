using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webapi.Models
{
    public class LoginModel
    {
        public string username { get; set; }
        public string pwd { get; set; }
    }

    public class RespAuthModel
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public DateTime expires { get; set; }
        public string username { get; set; }
        public string user_id { get; set; }        
        public string phone { get; set; }
    }

    public class RefreshTokenModel
    {
        public string expired_token { get; set; }
        public string refresh_token { get; set; }
        public string id { get; set; }
    }

    public class RespRefreshToken
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }
}
