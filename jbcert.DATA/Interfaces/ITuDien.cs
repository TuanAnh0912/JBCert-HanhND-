using jbcert.DATA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Interfaces
{
    public interface ITuDien
    {
        List<DanTocViewModel> GetDanTocs();

        List<GioiTinhViewModel> GetGioiTinhs();

        List<HinhThucCapViewModel> GetHinhThucCaps();

        List<string> GetTruongDuLieusThongTinHocSinh();

        List<TrangThaiPhoiViewModel> GetTrangThaiPhois();

        List<TrangThaiBangViewModel> GetTrangThaiBangs();

        List<XepLoaiTotNghiepViewModel> GetXepLoaiTotNghiepByCapDonVi(string codeCapDonVi);

        List<MauCongVanViewModel> GetMauCongVan(int loaiYeuCauId);

        List<CapDonViViewModel> GetAllCapDonVis();

        List<LoaiBangViewModel> GetLoaiBangTheoHinhThuc(int? hinhThucCapId);

        List<LoaiYeuCauViewModel> GetLoaiYeuCauMuaPhoi();

        List<int> GetNamTotNghiepsByTruong(int truongHocId);

        List<LoaiNhomTaoVanBangViewModel> GetLoaiNhomVanBangs();

        List<LopHocViewModel> GetLopHocByNam(string nam, int donViId);

        List<DonViViewModel> GetDonViByDonViCha(int donViChaId);

        List<DonViViewModel> GetTruongHocByDonViChaVaNhomTaoVanBang(int donViChaId);

        List<ThongBaoTypeViewModel> GetThongBaoTypes(int donViId);

        string GetCodeCapDonViByNguoiDung(int donViId);

        GetNamTotNghiepByDonViViewModel GetNamTotNghiepByDonVi(int donViId);

        List<DonViViewModel> GetAllDonViByDonViCha(int donViChaId);

        List<LopHocViewModel> GetLopHocByNamVaDonVi(int nam, int truongHocId, int donViId);

        DonViViewModel GetCodeCapDonViByDonViId(int donViId);

        List<CapDonViViewModel> GetCapDonVisForUpdating(int? currentDonViId, int donViId);

        List<DonViViewModel> GetCurrentDonVi(int? currentDonViId, int donViId);
    }
}
