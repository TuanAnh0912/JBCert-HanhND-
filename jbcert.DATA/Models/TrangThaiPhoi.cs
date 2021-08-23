using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class TrangThaiPhoi
    {
        public TrangThaiPhoi()
        {
            Phois = new HashSet<PhoiVanBang>();
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public string MaMau { get; set; }
        public string Border { get; set; }
        public string MauChu { get; set; }
        public virtual ICollection<PhoiVanBang> Phois { get; set; }
    }
}
