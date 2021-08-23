using System;
using System.Collections.Generic;
using System.Text;

namespace jbcert.DATA.ViewModels
{
    public class AddPhoiViewModel
    {
        public int LoaiBangId { get; set; }

        public int SoLuongPhoi { get; set; }

        public string TenNguoiLayPhoi { get; set; }

        public DateTime ThoiGianLayPhoi { get; set; }

        public string DiaDiemLayPhoi { get; set; }

        public Guid NguoiTao { get; set; }

        public DateTime NgayTao { get; set; }

        public Guid NguoiCapNhat { get; set; }

        public DateTime NgayCapNhat { get; set; }
    }
}
