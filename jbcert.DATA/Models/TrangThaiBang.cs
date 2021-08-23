using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class TrangThaiBang
    {
        public TrangThaiBang()
        {
            Bangs = new HashSet<Bang>();
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public string MaMauTrangThai { get; set; }

        public virtual ICollection<Bang> Bangs { get; set; }
    }
}
