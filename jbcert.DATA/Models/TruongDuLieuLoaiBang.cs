using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class TruongDuLieuLoaiBang
    {
        public int LoaiBangId { get; set; }
        public string TruongDuLieuCode { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public string Format { get; set; }
        public string Font { get; set; }
        public string Color { get; set; }
        public bool? Bold { get; set; }
        public bool? Italic { get; set; }
        public bool? Underline { get; set; }
        public int? Size { get; set; }
        public bool? DungChung { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhat { get; set; }
        public bool? IsDeleted { get; set; }
        public string TenTruongDuLieu { get; set; }
        public int KieuDuLieu { set; get; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int? DonViId { get; set; }
        public virtual LoaiBang LoaiBang { get; set; }
        public virtual TruongDuLieu TruongDuLieuCodeNavigation { get; set; }
    }
}
