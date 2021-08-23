using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace jbcert.DATA.IdentityModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }

        public Nullable<int> NhomNguoiDungId { get; set; }

        public Nullable<int> PhongBanId { get; set; }

        public Nullable<System.DateTime> NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string SoCanCuoc { get; set; }
        public bool LaGiaoVien { get; set; }
        public string DienThoai { get; set; }
        public Nullable<int> GioiTinhId { get; set; }
        public Nullable<int> DanTocId { get; set; }
        public Nullable<int> DonViId { get; set; }
    }
}
