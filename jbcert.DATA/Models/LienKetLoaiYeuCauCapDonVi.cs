using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class LienKetLoaiYeuCauCapDonVi
    {
        public int LienKetId { get; set; }
        public int? LoaiYeuCauId { get; set; }
        public int? CapDonViId { get; set; }

        public virtual CapDonVi CapDonVi { get; set; }
        public virtual LoaiYeuCau LoaiYeuCau { get; set; }
    }
}
