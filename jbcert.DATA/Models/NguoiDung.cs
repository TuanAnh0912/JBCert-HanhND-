using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class NguoiDung
    {
        public NguoiDung()
        {
            LogNguoiDungs = new HashSet<LogNguoiDung>();
        }

        public Guid NguoiDungId { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
        public int? NhomNguoiDungId { get; set; }
        public int? PhongBanId { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string SoCanCuoc { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public bool? Active { get; set; }
        public bool? IsDelete { get; set; }
        public int? GioiTinhId { get; set; }
        public int? DanTocId { get; set; }
        public int? DonViId { get; set; }
        public bool LaGiaoVien { get; set; }
        public virtual DonVi DonVi { get; set; }
        public virtual NhomNguoiDung NhomNguoiDung { get; set; }
        public virtual PhongBan PhongBan { get; set; }
        public virtual ICollection<LogNguoiDung> LogNguoiDungs { get; set; }
    }
}
