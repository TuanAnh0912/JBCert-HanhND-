using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class LogHocSinh
    {
        public int LogId { get; set; }
        public int? HocSinhId { get; set; }
        public string HanhDong { get; set; }
        public DateTime? ThoiGian { get; set; }
        public Guid? NguoiDungId { get; set; }
        public string HoTen { get; set; }

        public string Ip { get; set; }

        public virtual HocSinh HocSinh { get; set; }
    }
}
