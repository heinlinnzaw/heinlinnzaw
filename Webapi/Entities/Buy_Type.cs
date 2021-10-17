using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Webapi.Entities
{
    [Table("Buy_Type")]
    public class Buy_Type
    {
        [Key]
        public string type_id { get; set; }
        public string type { get; set; }
        public int gift_per_user_limit { get; set; }
    }
}
