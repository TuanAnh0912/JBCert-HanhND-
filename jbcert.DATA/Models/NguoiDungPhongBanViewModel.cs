using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class NguoiDungPhongBanViewModel
    {
        public string HoTen { get; set; }

        public string TenDangNhap { get; set; }

        public int? DonViId { get; set; }

        // phong ban
        public int? PhongBanId { get; set; }
        public string TenPhongBan { get; set; }
        public string Mota { get; set; }

    }
}
