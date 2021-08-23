using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class BangViewModel
    {
        public int Id { get; set; }
        public int? LoaiBangId { get; set; }
        public string HoVaTen { get; set; }
        public string SoVaoSo { get; set; }
        public string SoHieu { get; set; }
        public string TruongHoc { get; set; }
        public string TenLoaiBang { get; set; }
        public int? AnhLoaiBangWidth { get; set; }
        public int? AnhLoaiBangHeight { get; set; }
        public int? PhoiId { get; set; }
        public int? TrangThaiBangId { get; set; }
        public string TrangThaiBang { get; set; }
        public string MaMauTrangThaiBang { get; set; }
        public int? HocSinhId { get; set; }
        public int? TruongHocId { get; set; }
        public int? YeuCauId { get; set; }
        public string AnhLoaiBang { get; set; }
        public string DuongDanFileAnh { get; set; }
        public string DuongDanFileDeIn { get; set; }
        public string CmtnguoiLayBang { get; set; }
        public string SoDienThoaiNguoiLayBang { get; set; }
        public string QuanHeVoiNguoiDuocCapBang { get; set; }
        public string HinhThucNhan { get; set; }
        public string HoKhauThuongTru { get; set; }
        public string LopHoc { get; set; }
        public string GioiTinh { get; set; }
        public string DanToc { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string HinhThucDaoTao { get; set; }
        public string DiemThi { get; set; }
        public bool? XetHK { get; set; }
        public int? UT { get; set; }
        public string KK { get; set; }
        public string HL { get; set; }
        public string HK { get; set; }
        public bool? KQ { get; set; }
        public int? SoLanXet { get; set; }
        public int? DonViId { get; set; }
        public string XepLoaiTotNghiep { get; set; }
        public int? NhomTaoVanBangId { get; set; }
        public int? SoLanCaiChinh { get; set; }
        public int? BangCuId { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhat { get; set; }
        public int? BangGocId { get; set; }
        public bool? IsDeleted { get; set; }
        public int? NamTotNghiep { get; set; }
        public DateTime? NgayInBang { get; set; }
        public DateTime? NgayPhatBang { get; set; }
        public int? IsBanSao { get; set; }
        public bool? IsChungChi { get; set; }
        public List<TruongDuLieuTrongBangViewModel> TruongDuLieuTrongBangs { get; set; }
        public List<ThongTinVBViewModel> ThongTinVanBangs { get; set; }
        public List<AnhVanBangViewModel> AnhVanBangs { get; set; }
        public List<LogVanBangViewModel> Logs { get; set; }
        public List<LogCaiChinhViewModel> LogCaiChinhs { get; set; }
        public List<CaiChinhViewModel> CaiChinhs { get; set; }
    }
}
