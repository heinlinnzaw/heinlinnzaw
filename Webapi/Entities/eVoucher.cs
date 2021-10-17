using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Webapi.Entities
{
    [Table("evoucher")]
    public class eVoucher
    {
        [Key]
        public string ev_Id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime expiry_date { get; set; }
        public byte[] image { get; set; }
        public decimal amount { get; set; }
        public int qty { get; set; }
        public bool is_active { get; set; }
        public string filename { get; set; }
        public string fileextension { get; set; }
        public DateTime? created_on { get; set; }
        public DateTime? updated_on { get; set; }
    }
}
