﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class BangCuViewModel
    {
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
        public string HinhThucNhan { get; set; }
        public int? DonViId { get; set; }
        public DateTime? NgayInBang { get; set; }
        public DateTime? NgayPhatBang { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhat { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
