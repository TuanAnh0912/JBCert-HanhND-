﻿using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class LogNguoiDung
    {
        public int LogId { get; set; }
        public Guid? NguoiDungId { get; set; }
        public string HanhDong { get; set; }
        public DateTime? ThoiGian { get; set; }
        public string Ip { get; set; }

        public virtual NguoiDung NguoiDung { get; set; }
    }
}
