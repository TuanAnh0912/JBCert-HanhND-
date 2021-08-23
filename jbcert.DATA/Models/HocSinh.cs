using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class HocSinh
    {
        public HocSinh()
        {
            HocSinhFileDinhKems = new HashSet<HocSinhFileDinhKem>();
            LienKetHocSinhYeuCaus = new HashSet<LienKetHocSinhYeuCau>();
            LogHocSinhs = new HashSet<LogHocSinh>();
        }

        public int Id { get; set; }
        public int? TT { get; set; }
        public string HoVaTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string NoiSinh { get; set; }
        public string HoKhauThuongTru { get; set; }
        public int? TruongHocId { get; set; }
        public int? DanTocId { get; set; }
        public int? GioiTinhId { get; set; }
        public int? NamTotNghiep { get; set; }
        public string XepLoaiTotNghiep { get; set; }
        public string HinhThucDaoTao { get; set; }
        //public string DiemThi { get; set; }
        public string SoVaoSo { get; set; }
        public string DanToc { get; set; }
        public string GioiTinh { get; set; }
        public string TruongHoc { get; set; }
        public string LopHoc { get; set; }
        public int? LopHocId { get; set; }
        public bool? XetHK { get; set; }
        public int? UT { get; set; }
        public string KK { get; set; }
        public string HL { get; set; }
        public string HK { get; set; }
        public bool? KQ { get; set; }
        public bool? CongNhanTotNghiep { get; set; }
        public int? SoLanXet { get; set; }
        public bool? DaInBangGoc { get; set; }
        public string HoiDongThi { get; set; }
        public string DiemThi { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhat { get; set; }
        public bool? IsDeleted { get; set; }
        public int DonViId { get; set; }
        public virtual DanToc DanTocNavigation { get; set; }
        public virtual GioiTinh GioiTinhNavigation { get; set; }
        public virtual DonVi TruongHocNavigation { get; set; }
        public virtual ICollection<HocSinhFileDinhKem> HocSinhFileDinhKems { get; set; }
        public virtual ICollection<LienKetHocSinhYeuCau> LienKetHocSinhYeuCaus { get; set; }
        public virtual ICollection<LogHocSinh> LogHocSinhs { get; set; }
    }
}
