using System;
using System.Collections.Generic;
using System.Text;

namespace jbcert.DATA.ViewModels
{
    public class DanhSachVanBangViewModel
    {
        public List<HocSinhTaoVanBangViewModel> VanBangs { get; set; }

        public int? TotalPage { get; set; }

        public int? CurrentPage { get; set; }

        public int? NhomTaoVanBangId { get; set; }

        public int? TrangThaiBangId { get; set; }

        public bool? ChoPhepTaoLai { get; set; }
    }
}
