using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class LienKetCapDonViChucNang
    {
        public int LienKetId { get; set; }
        public int? CapDonViId { get; set; }
        public int? ChucNangId { get; set; }

        public virtual CapDonVi CapDonVi { get; set; }
        public virtual ChucNang ChucNang { get; set; }
    }
}
