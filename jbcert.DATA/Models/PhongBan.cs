using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class PhongBan
    {
        public PhongBan()
        {
            NguoiDungs = new HashSet<NguoiDung>();
            NhomNguoiDungs = new HashSet<NhomNguoiDung>();
        }

        public int PhongBanId { get; set; }
        public string TenPhongBan { get; set; }

        public string Mota { get; set; }

        public int? DonViId { get; set; }
        public bool? IsDelete { get; set; }
        public virtual DonVi DonVi { get; set; }
        public virtual ICollection<NguoiDung> NguoiDungs { get; set; }
        public virtual ICollection<NhomNguoiDung> NhomNguoiDungs { get; set; }
    }
}
