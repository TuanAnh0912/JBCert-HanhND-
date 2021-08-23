using jbcert.DATA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Interfaces
{
    public interface IThongTinVanBang
    {
        DetailThongTinBangViewModel DetailThongTinBang(int bangId, int donViId);

        void UpdateThongTinVanBang(List<TruongDuLieuTrongBangViewModel> truongDuLieuTrongBangViewModels, int donViId);

        void TaoAnhVanBangTheoYeuCau(List<TaoAnhVanBangTheoYeuCauViewModel> inVanBangViewModels, int donViId);

        List<int> CapNhatTrangThaiPhoiVaBangSauKhiIn(List<TrangThaiPhoiVaBangSauKhiInViewModel> trangThaiPhoiVaBangSauKhiInViewModels);

        List<BangViewModel> DownLoadFileAnh(TaoAnhVanBangViewModel taoAnhVanBangViewModel);

        DanhSachVanBangViewModel DanhSachBang(string hoVaTen, string soVaoSo, string lopHoc, int loaiBangId, List<int> truongIds,int donViId, int currentPage);

        void PhatVanBang(PhatVanBangViewModel phatVanBangViewModel, int donViId);

        DanhSachSoGocViewModel GetSoGocs(int? truongHocId, string hoVaTen, int? namTotNghiep, int? donViId, int currentPage);

        DanhSachVanBangViewModel GetVanBangs(int? truongHocId, int? loaiBangId, string hoVaTen, string nienKhoa, int? trangThaiBangId, int? donViId, int currentPage);

        DanhSachBangViewModel GetDanhSachHocSinhDeTaoAnhVanBang(int nhomTaoVanBangId, string hoVaTen, int? donViId, int currentPage);

        DanhSachBangViewModel GetDanhSachHocSinhDeCapNhatSoHieu(int nhomTaoVanBangId, string hoVaTen, int? trangThaiBangId ,int? donViId, int currentPage);

        DanhSachHocSinhTaoVanBangViewModel GetHocSinhTaoVanBang(int nhomTaoVanBangId, string hoVaTen, int donViId, int currentPage);

        void AddThongTinVanBang(AddThongTinVanBangViewModel addThongTinVanBangViewModel, int donViId, string ip);

        void TaoAnhVanBang(TaoAnhVanBangViewModel taoAnhVanBangViewModel, int donViId);

        void DeleteNhomTaoVanBang(int nhomTaoVanBangId, int donViId);

        void UpdateTrangThaiDaIn(HocSinhTaoVanBangViewModel hocSinhTaoVanBangViewModel, int donViId, string ip);

        void UpdateTrangThaiPhatBang(PhatVanBangViewModel phatVanBangViewModel, int donViId, string ip);

        ChiTietThongTinVanBangViewModel<BangViewModel, LogVanBangViewModel, LogThongTinCaiChinhViewModel> ChiTietThongTinVanBang(int bangId, int donViId);

        void TaoBanSaoTuBangGoc(HocSinhTaoVanBangViewModel hocSinhTaoVanBangViewModel, int donViId, string ip);

        void TaoAnhBanSao(TaoAnhVanBangViewModel taoAnhVanBangViewModel, int donViId);

        void AddNhomTaoVanBang(NhomTaoVanBangViewModel nhomTaoVanBangViewModel,int donViId);

        NhomTaoVanBangViewModel GetNhomTaoVanBang(int nhomTaoVanBangId, int donViId);

        NhomTaoVanBangsViewModel GetNhomTaoVanBangs(int? truongHocId, int? LoaiBangId, int? loaiNhomTaoVanBangId, int? trangThaiBangId, int? nam, int donViId, bool? isChungChi);

        NhomTaoVanBangWithPaginationViewModel GetNhomTaoVanBangDaIns(int? truongHocId, int? LoaiBangId, int? loaiNhomTaoVanBangId, int? trangThaiBangId, int? nam, bool? isChungChi, int donViId, int currentPage);

        ChiTietSoGocViewModel GetChiTietSoGoc(int bangId, int donViId);

        DanhSachBangViewModel GetDanhSachBangChoTruong(int? trangThaiBangId, int? lopHocId, string hoVaTen, int? namTotNghiep, int currentPage, int donViId);

        void ChuyenDonViSoIn(ChuyenDonViInViewModel chuyenDonViInViewModel, int donViId);

        void ChuyenDonViPhongIn(ChuyenDonViInViewModel chuyenDonViInViewModel, int donViId);
    }
}
