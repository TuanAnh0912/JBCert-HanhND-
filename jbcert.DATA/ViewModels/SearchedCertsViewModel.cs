using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class SearchedCertsViewModel
    {
        public int Id { get; set; }
        public int? LoaiBangId { get; set; }
        public string AnhLoaiBang { get; set; }
        public string HoVaTen { get; set; }
        public string LoaiBang { get; set; }
        public string SoHieu { get; set; }
        public string SoVaoSo { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? PhoiId { get; set; }
        public int? TrangThaiBangId { get; set; }
        public int? HocSinhId { get; set; }
        public int? TruongHocId { get; set; }
        public string TruongHoc { get; set; }
        public int? YeuCauId { get; set; }
        public string DuongDanFileAnh { get; set; }
        public string DuongDanFileDeIn { get; set; }
        public string CmtnguoiLayBang { get; set; }
        public string SoDienThoaiNguoiLayBang { get; set; }
        public string HinhThucNhan { get; set; }
        public int? DonViId { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhat { get; set; }
        public DateTime? NgayInBang { get; set; }
        public DateTime? NgayPhatBang { get; set; }
        public int? BangGocId { get; set; }
        public bool? IsDeleted { get; set; }
        public string GiaTri { get; set; }
        public int? KieuDuLieu { get; set; }
        public int? X { get; set; }

        public int? Y { get; set; }

        public string Color { get; set; }

        public string Font { get; set; }

        public bool? Italic { get; set; }

        public bool? Bold { get; set; }

        public bool? Underline { get; set; }

        public int? Size { get; set; }

        public string TruongDuLieuCode { get; set; }
        public string TenTruongDuLieu { get; set; }

        public string TenTruongDuLieuHocSinh { get; set; }

        public List<TruongDuLieuTrongBangViewModel> TruongDuLieuTrongBangs { get; set; }

        public HocSinhViewModel HocSinh { get; set; }

        public List<LogVanBangViewModel> LogVanBangs { get; set; }

    }
}
