using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Webapi.Entities
{
    [Table("purchase")]
    public class Purchase
    {
        [Key]
        public string Id { get; set; }
        public string customer_id { get; set; }
        public string buy_type_id { get; set; }
        public string ev_id { get; set; }
        public string promocode { get; set; }
        public string customername { get; set; }
        public string phoneno { get; set; }
        public string payment_type_id { get; set; }
        public int discount { get; set; }
        public DateTime expiry_date { get; set; }
        public int qty { get; set; }
        public DateTime created_on { get; set; }
        public bool is_verify { get; set; }
        public DateTime? verify_date { get; set; }
        public decimal balance { get; set; }
        public byte[] qrcode { get; set; }
    }
}
