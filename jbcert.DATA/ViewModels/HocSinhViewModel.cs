using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class HocSinhViewModel
    {
        public int Id { get; set; }

        public int? TT { get; set; }

        public string HoVaTen { get; set; }

        public string Ho { get; set; }

        public string Ten { get; set; }

        public Nullable<System.DateTime> NgaySinh { get; set; }

        public string NoiSinh { get; set; }

        public string HoKhauThuongTru { get; set; }

        public int TruongHocId { get; set; }

        public string TruongHoc { get; set; }

        public int? LopHocId { get; set; }

        public string LopHoc { get; set; }

        public string NienKhoa { get; set; }

        public int? DanTocId { get; set; }

        public string DanToc { get; set; }

        public int? GioiTinhId { get; set; }

        public string GioiTinh { get; set; }

        public int? NamTotNghiep { get; set; }

        public string XepLoaiTotNghiep { get; set; }

        public string HinhThucDaoTao { get; set; }

        public string SoVaoSo { get; set; }

        public bool? XetHK { get; set; }

        public int? UT { get; set; }

        public string KK { get; set; }

        public string HL { get; set; }

        public string HK { get; set; }

        public bool? KQ { get; set; }

        public bool? CongNhanTotNghiep { get; set; }

        public int? SoLanXet { get; set; }

        public bool? DaInBangGoc { get; set; }

        public int TrangThaiBangId { get; set; }

        public string TrangThaiBang { get; set; }

        public string MaMauTrangThaiBang { get; set; }

        public string DiemThi { get; set; }

        public string HoiDongThi { get; set; }

        public List<DiemMonHocViewModel> DiemMonHocs { get; set; }

        public DateTime NgayTao { get; set; }

        public Guid NguoiTao { get; set; }

        public DateTime NgayCapNhat { get; set; }

        public Guid NguoiCapNhat { get; set; }

        public bool IsDeleted { get; set; }

        public int DonViId { get; set; }

        public List<AttachFileViewModel> AttachFiles { get; set; }
        public List<HocSinhFileDinhKemViewModel> Files { get; set; }
    }
}
