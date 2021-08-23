using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class LogPhoi
    {
        [Key]
        public int LogId { get; set; }
        public Guid? NguoiDungId { get; set; }
        public string HanhDong { get; set; }
        public int? PhoiId { get; set; }
        public DateTime? ThoiGian { get; set; }
        public string HoTen { get; set; }
        public string Ip { get; set; }
    }
}
