using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Webapi.Entities
{
    [Table("customer")]
    public class Customer
    {
        [Key]
        public string Id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string pwd { get; set; }        
        public DateTime? last_login_datetime { get; set; }
    }
}
