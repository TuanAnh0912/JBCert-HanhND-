using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class TruongDuLieu
    {
        public TruongDuLieu()
        {
            ThongTinVanBangs = new HashSet<ThongTinVanBang>();
            TruongDuLieuLoaiBangs = new HashSet<TruongDuLieuLoaiBang>();
        }

        public string Code { get; set; }
        public string Ten { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhat { get; set; }
        public int KieuDuLieu { set; get; }
        public bool? IsDeleted { get; set; }
        public bool? IsChungChi { get; set; }
        public int DonViId { get; set; }

        public virtual ICollection<ThongTinVanBang> ThongTinVanBangs { get; set; }
        public virtual ICollection<TruongDuLieuLoaiBang> TruongDuLieuLoaiBangs { get; set; }
    }
}
