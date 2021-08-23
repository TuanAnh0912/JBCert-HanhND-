using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class LogVanBang
    {
        public int LogId { get; set; }
        public Guid? NguoiDungId { get; set; }
        public int? VanBangId { get; set; }
        public string HanhDong { get; set; }
        public DateTime? ThoiGian { get; set; }
        public string HoTen { get; set; }

        public string Ip { get; set; }

        public virtual Bang VanBang { get; set; }
    }
}
