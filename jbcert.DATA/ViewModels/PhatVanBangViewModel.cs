using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class PhatVanBangViewModel
    {
        public string CMTNguoiLayBang { get; set; }

        public string SoDienThoaiNguoiLayBang { get; set; }

        public string QuanHeVoiNguoiDuocCapBang { get; set; }

        public string HinhThucNhan { get; set; }

        public int BangId { get; set; }

        public int HocSinhId { get; set; }

        public bool? DaCap { get; set; }

        public int? NhomTaoVanBangId { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public Guid? NguoiCapNhat { get; set; }
    }
}
