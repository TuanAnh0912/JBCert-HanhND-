using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class Bang
    {
        public Bang()
        {
            LogVanBangs = new HashSet<LogVanBang>();
            ThongTinVanBangs = new HashSet<ThongTinVanBang>();
        }

        public int Id { get; set; }
        public int? LoaiBangId { get; set; }
        public int? PhoiId { get; set; }
        public int? TrangThaiBangId { get; set; }
        public int? HocSinhId { get; set; }
        public int? TruongHocId { get; set; }
        public int? YeuCauId { get; set; }
        public string SoVaoSo { get; set; }
        public string TruongHoc { get; set; }
        public string SoHieu { get; set; }
        public string HoVaTen { get; set; }
        public string DuongDanFileAnh { get; set; }
        public string DuongDanFileDeIn { get; set; }
        public string CmtnguoiLayBang { get; set; }
        public string SoDienThoaiNguoiLayBang { get; set; }
        public string QuanHeVoiNguoiDuocCapBang { get; set; }
        public string HinhThucNhan { get; set; }
        public int? DonViId { get; set; }
        public DateTime? NgayInBang { get; set; }
        public DateTime? NgayPhatBang { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhat { get; set; }
        public int? NamTotNghiep { get; set; }
        public int? BangGocId { get; set; }
        public int? SoLanCaiChinh { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsChungChi { get; set; }
        public string DiemThi { get; set; }
        public string HoiDongThi { get; set; }
        public int? SoHoaId { get; set; }
        public virtual PhoiVanBang Phoi { get; set; }
        public virtual TrangThaiBang TrangThaiBang { get; set; }
        public virtual ICollection<LogVanBang> LogVanBangs { get; set; }
        public virtual ICollection<ThongTinVanBang> ThongTinVanBangs { get; set; }
    }
}
