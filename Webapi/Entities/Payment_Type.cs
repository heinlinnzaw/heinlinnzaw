using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Webapi.Entities
{
    [Table("payment_type")]
    public class Payment_Type
    {
        [Key]
        public string payment_id { get; set; }
        public string payment_type { get; set; }
        public int discount { get; set; }
    }
}
