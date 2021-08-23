using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class LoaiBang
    {
        public LoaiBang()
        {
            TruongDuLieuLoaiBangs = new HashSet<TruongDuLieuLoaiBang>();
        }

        public int Id { get; set; }
        public string Ten { get; set; }
        public int? HinhThucCapId { get; set; }
        public string MaLoaiBang { get; set; }
        public string MaNoiIn { get; set; }
        public string CodeCapDonVi { get; set; }
        public int? ChiSo { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhat { get; set; }
        public int? LoaiBangGocId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsChungChi { get; set; }
        public int? DonViId { get; set; }
        public virtual HinhThucCap HinhThucCap { get; set; }
        public virtual ICollection<TruongDuLieuLoaiBang> TruongDuLieuLoaiBangs { get; set; }
    }
}
