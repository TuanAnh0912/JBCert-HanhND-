using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class CapDonVi
    {
        public CapDonVi()
        {
            DonVis = new HashSet<DonVi>();
            LienKetCapDonViChucNangs = new HashSet<LienKetCapDonViChucNang>();
            LienKetLoaiYeuCauCapDonVis = new HashSet<LienKetLoaiYeuCauCapDonVi>();
        }

        public int CapDonViId { get; set; }
        public string TenCapDonVi { get; set; }
        public string Code { get; set; }
        public int Level { get; set; }
        public virtual ICollection<DonVi> DonVis { get; set; }
        public virtual ICollection<LienKetCapDonViChucNang> LienKetCapDonViChucNangs { get; set; }
        public virtual ICollection<LienKetLoaiYeuCauCapDonVi> LienKetLoaiYeuCauCapDonVis { get; set; }
    }
}
