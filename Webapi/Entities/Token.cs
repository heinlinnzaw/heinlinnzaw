using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Webapi.Entities
{
    [Table("token")]
    public partial class TokenEntity
    {   
        [Key]
        public string Token_Id { get; set; }
        
        public string Customer_Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool Is_Active { get; set; }
        public DateTime? Created_On { get; set; }
        public DateTime? Revoked_On { get; set; }
        public string Refresh_Token { get; set; }
    }
}
