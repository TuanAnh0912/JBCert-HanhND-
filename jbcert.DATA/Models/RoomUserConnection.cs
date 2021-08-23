using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class RoomUserConnection
    {
        [Key]
        public int Id { get; set; }

        public string RoomName { get; set; }

        public string UserId { get; set; }

        public int? PhongBanId { get; set; }

        public int? DonViId { get; set; }
    }
}
