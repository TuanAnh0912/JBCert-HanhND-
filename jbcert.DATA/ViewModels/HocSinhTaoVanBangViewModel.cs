using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class HocSinhTaoVanBangViewModel
    {
        public int YeuCauId { get; set; }

        public int? BangId { get; set; }

        public int? NhomTaoVanBangId { get; set; }

        public int LoaiBangWidth { get; set; }
        public int LoaiBangHeight { get; set; }

        public int PhoiId { get; set; }

        public string SoHieu { get; set; }

        public int Id { get; set; }

        public int LoaiBangId { get; set; }

        public string LoaiBang { get; set; }

        public string HoVaTen { get; set; }

        public DateTime? NgaySinh { get; set; }

        public string NoiSinh { get; set; }

        public string HoKhauThuongTru { get; set; }

        public int TruongHocId { get; set; }

        public string TruongHoc { get; set; }

        public int DanTocId { get; set; }

        public string LopHoc { get; set; }

        public string DanToc { get; set; }

        public int GioiTinhId { get; set; }

        public string GioiTinh { get; set; }

        public int? NamTotNghiep { get; set; }

        public string XepLoaiTotNghiep { get; set; }

        public string HinhThucDaoTao { get; set; }

        public string DiemThi { get; set; }

        public string SoVaoSo { get; set; }

        public bool? XetHK { get; set; }

        public bool? UT { get; set; }

        public bool? KK { get; set; }

        public string HL { get; set; }

        public string HK { get; set; }

        public bool? KQ { get; set; }

        public bool? CongNhanTotNghiep { get; set; }

        public int? SoLanXet { get; set; }

        public bool? DaInBangGoc { get; set; }

        public DateTime? NgayInBang { get; set; }

        public DateTime? NgayPhatBang { get; set; }

        public DateTime NgayTao { get; set; }

        public Guid NguoiTao { get; set; }

        public DateTime NgayCapNhat { get; set; }

        public Guid NguoiCapNhat { get; set; }

        public int? BangGocId { get; set; }

        public bool IsDeleted { get; set; }

        public int DonViId { get; set; }

        public string DuongDanFileAnh { get; set; }
        public string DuongDanFileAnhDeIn { get; set; }
        public DateTime? NgayTaoBang { get; set; }

        public int TrangThaiBangId { get; set; }

        public string TrangThaiBang { get; set; }

        public string MaMauTrangThaiBang { get; set; }

        public string CMTNguoiLayBang { get; set; }

        public string SoDienThoaiNguoiLayBang { get; set; }
        public string HinhThucNhan { get; set; }

        public int? SoLuongBanSao { get; set; }

        public int? BangCuId { get; set; }
        public List<TruongDuLieuTrongBangViewModel> TruongDuLieuTrongBangs { get; set; }

    }
}
