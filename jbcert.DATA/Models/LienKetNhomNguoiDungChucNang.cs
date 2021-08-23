using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class LienKetNhomNguoiDungChucNang
    {
        public int LienKetId { get; set; }
        public int NhomNguoiDungId { get; set; }
        public int? ChucNangid { get; set; }

        public virtual ChucNang ChucNang { get; set; }
        public virtual NhomNguoiDung NhomNguoiDung { get; set; }
    }
}
