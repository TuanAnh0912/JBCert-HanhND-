using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class PhoiVanBang
    {
        public PhoiVanBang()
        {
            Bangs = new HashSet<Bang>();
        }

        public int Id { get; set; }
        public int? LoaiBangId { get; set; }
        public string SoHieu { get; set; }
        public string ChiSoCoDinh { get; set; }
        public string ChiSoThayDoi { get; set; }
        public int? TrangThaiPhoiId { get; set; }
        public string MoTaTrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public bool? IsDeleted { get; set; }
        public int? DonViId { get; set; }
        public string DonViDaNhan { get; set; }
        public int? NhapPhoiId { get; set; }
        public bool? DaCap { get; set; }
        public virtual TrangThaiPhoi TrangThaiPhoi { get; set; }
        public virtual ICollection<Bang> Bangs { get; set; }
    }
}
