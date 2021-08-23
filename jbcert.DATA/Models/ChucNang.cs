using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class ChucNang
    {
        public ChucNang()
        {
            LienKetCapDonViChucNangs = new HashSet<LienKetCapDonViChucNang>();
            LienKetNhomNguoiDungChucNangs = new HashSet<LienKetNhomNguoiDungChucNang>();
        }

        public int ChucNangId { get; set; }
        public int? KhoaChaId { get; set; }
        public string TenChucNang { get; set; }
        public string AuthCode { get; set; }
        public bool? ShowOnMenu { get; set; }
        public string Icon { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string DefaultAlias { get; set; }
        public int? Order { get; set; }
        public virtual ICollection<LienKetCapDonViChucNang> LienKetCapDonViChucNangs { get; set; }
        public virtual ICollection<LienKetNhomNguoiDungChucNang> LienKetNhomNguoiDungChucNangs { get; set; }
    }
}
