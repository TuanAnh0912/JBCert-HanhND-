using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class GioiTinh
    {
        public GioiTinh()
        {
            HocSinhs = new HashSet<HocSinh>();
        }

        public int Id { get; set; }
        public string Ten { get; set; }

        public virtual ICollection<HocSinh> HocSinhs { get; set; }
    }
}
