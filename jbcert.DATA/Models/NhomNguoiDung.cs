using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class NhomNguoiDung
    {
        public NhomNguoiDung()
        {
            LienKetNhomNguoiDungChucNangs = new HashSet<LienKetNhomNguoiDungChucNang>();
            NguoiDungs = new HashSet<NguoiDung>();
        }

        public int NhomNguoiDungId { get; set; }
        public string TenNhomNguoiDung { get; set; }
        public int? PhongBanId { get; set; }
        public virtual PhongBan PhongBan { get; set; }
        public virtual ICollection<LienKetNhomNguoiDungChucNang> LienKetNhomNguoiDungChucNangs { get; set; }
        public virtual ICollection<NguoiDung> NguoiDungs { get; set; }
    }
}
