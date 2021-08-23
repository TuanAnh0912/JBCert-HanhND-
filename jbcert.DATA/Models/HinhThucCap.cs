using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class HinhThucCap
    {
        public HinhThucCap()
        {
            LoaiBangs = new HashSet<LoaiBang>();
        }

        public int Id { get; set; }
        public string Ten { get; set; }

        public virtual ICollection<LoaiBang> LoaiBangs { get; set; }
    }
}
