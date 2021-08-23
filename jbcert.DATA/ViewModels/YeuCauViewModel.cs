using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class YeuCauViewModel
    {
        public int Id { get; set; }
        public string NoiDung { get; set; }
        public string NguoiTaoYeuCau { get; set; }
        public Nullable<int> DonViYeuCauId { get; set; }
        public string TenDonViYeuCau { get; set; }
        public Nullable<int> LoaiYeuCauId { get; set; }
        public string TenLoaiYeuCau { get; set; }
        public Nullable<int> DonViDichId { get; set; }
        public string TenDonViDich { get; set; }
        public string DiaGioiHanhChinhDonViDich { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<System.Guid> NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public Nullable<System.Guid> NguoiCapNhat { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> DonViId { get; set; }
        public string TenDonVi { get; set; }
        public string MaTrangThaiYeuCau { get; set; }
        public string TenTrangThai { get; set; }
        public string GhiChu { get; set; }
        public string MaYeuCau { get; set; }
        public string TenYeuCau { get; set; }
        public string MaMau { get; set; }
        public string MauChu { get; set; }
        public string Border { get; set; }
        public int TrangThaiBangId { get; set; }
        public string TrangThaiBang { get; set; }
        public string MaMauTrangThai { get; set; }
        public Nullable<int> LoaiVanBangId { get; set; }
        public string TenLoaiBang { get; set; }
        public Nullable<int> DonViChuyenTiepId { get; set; }
        public Nullable<System.DateTime> NgayGuiYeuCau { get; set; }
        public List<LogYeuCauViewModel> Logs { get; set; }
        public List<FileDinhKemYeuCauViewModel> Files { get; set; }
        public List<HocSinhViewModel> HocSinhs { get; set; }
        public List<FileHocSinhYeuCauDuXetTotNghiepViewModel> FileHocSinhYeuCauDuXetTotNghieps { get; set; }

        public List<string> DuongDanFiles { get; set; }
    }
}
