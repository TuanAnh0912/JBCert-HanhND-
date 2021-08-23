using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class LogVanBangViewModel
    {
        public int LogId { get; set; }
        public Guid? NguoiDungId { get; set; }
        public int? VanBangId { get; set; }
        public string HanhDong { get; set; }
        public DateTime? ThoiGian { get; set; }
        public string HoTen { get; set; }
        public string Ip { get; set; }
    }
}
