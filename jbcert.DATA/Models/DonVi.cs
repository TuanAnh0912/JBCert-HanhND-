using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class DonVi
    {
        public DonVi()
        {
            HocSinhs = new HashSet<HocSinh>();
            NguoiDungs = new HashSet<NguoiDung>();
            PhongBans = new HashSet<PhongBan>();
        }

        public int DonViId { get; set; }
        public string TenDonVi { get; set; }
        public int? KhoaChaId { get; set; }
        public int? CapDonViId { get; set; }
        public string MaDonVi { get; set; }
        public string DiaGioiHanhChinh { get; set; }

        public virtual CapDonVi CapDonVi { get; set; }
        public virtual ICollection<HocSinh> HocSinhs { get; set; }
        public virtual ICollection<NguoiDung> NguoiDungs { get; set; }
        public virtual ICollection<PhongBan> PhongBans { get; set; }
    }
}
