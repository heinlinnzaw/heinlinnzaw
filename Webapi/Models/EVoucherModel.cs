using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webapi.Models
{
    public class EVoucherModel
    {
        public string ev_Id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime expiry_date { get; set; }
        public byte[] image { get; set; }
        public string filename { get; set; }
        public string fileextension { get; set; }
        public IFormFile uploadimage { get; set; }
        public decimal amount { get; set; }
        public int qty { get; set; }
        public bool is_active { get; set; }
        public DateTime created_on { get; set; }
        public DateTime updated_on { get; set; }
        public string status { get; set; }
    }

    public class RespQRCode
    {
        public string fileextension { get { return "image/png"; } }
        public byte[] qr { get; set; }
    }

    public class VerifyPromoCodeModel
    {
        public string phone { get; set; }
        public string promocode { get; set; }
    }
}
