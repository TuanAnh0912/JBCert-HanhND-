using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class NguoiDungViewModel
    {
        public System.Guid NguoiDungId { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
        public Nullable<int> NhomNguoiDungId { get; set; }
        public string TenNhomNguoiDung { get; set; }
        public Nullable<int> PhongBanId { get; set; }
        public string TenDonVi { get; set; }
        public string TenPhongBan { get; set; }
        public Nullable<System.DateTime> NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string SoCanCuoc { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<System.Guid> NguoiTao { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<int> GioiTinhId { get; set; }
        public Nullable<int> DanTocId { get; set; }
        public Nullable<int> DonViId { get; set; }
        public bool LaGiaoVien { get; set; }
        public List<LopHocViewModel> LopHocs { get; set; }
    }
}
