using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class LoaiYeuCau
    {
        public LoaiYeuCau()
        {
            LienKetLoaiYeuCauCapDonVis = new HashSet<LienKetLoaiYeuCauCapDonVi>();
            YeuCaus = new HashSet<YeuCau>();
        }

        public int Id { get; set; }
        public string TenLoaiYeuCau { get; set; }

        public virtual ICollection<LienKetLoaiYeuCauCapDonVi> LienKetLoaiYeuCauCapDonVis { get; set; }
        public virtual ICollection<YeuCau> YeuCaus { get; set; }
    }
}
