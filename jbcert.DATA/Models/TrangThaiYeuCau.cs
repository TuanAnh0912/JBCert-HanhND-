using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class TrangThaiYeuCau
    {
        public TrangThaiYeuCau()
        {
            YeuCaus = new HashSet<YeuCau>();
        }

        public string Code { get; set; }
        public string TenTrangThai { get; set; }
        public string MaMau { get; set; }
        public string MauChu { get; set; }
        public string Border { get; set; }

        public virtual ICollection<YeuCau> YeuCaus { get; set; }
    }
}
