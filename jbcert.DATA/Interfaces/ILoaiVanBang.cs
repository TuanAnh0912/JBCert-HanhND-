using jbcert.DATA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Interfaces
{
    public interface ILoaiVanBang
    {
        List<TruongDuLieuViewModel> GetTruongDuLieus(bool? isChungChi, int donViId);

        TruongDuLieuViewModel GetTruongDuLieu(string code, int donViId);

        void AddTruongDuLieu(TruongDuLieuViewModel truongDuLieu);

        void UpdateTruongDuLieu(TruongDuLieuViewModel truongDuLieu);

        void DeleteTruongDuLieu(TruongDuLieuViewModel truongDuLieu);

        LoaiBangViewModel GetLoaiBang(int loaiBangId, int donViId);

        LoaiBangsWithPaginationViewModel GetLoaiBangs(string ten, int? hinhThucCapId, bool? isChungChi, int currentPage, int donViId);
        List<LoaiBangViewModel> GetLoaiBangTheoCapDonVis(string ten, int? hinhThucCapId, int donViId);

        List<LoaiBangViewModel> GetLoaiBangs();

        void AddLoaiBang(LoaiBangViewModel loaiBangViewModel, int donViId);

        void UpdateLoaiBang(LoaiBangViewModel loaiBangViewModel, int donViId);

        void DeleteLoaiBang(LoaiBangViewModel loaiBangViewModel, int donViId);

        void AddAnhLoaiBang(List<LoaiBangFileDinhKemViewModel> files, int donViId);

        void AddAnhLoaiBangCu(AttachFileViewModel attachFileViewModel, int donViId);

        LoaiBangFileDinhKemViewModel GetAnhLoaiBang(int loaiBangId, int donViId);

        AttachFileViewModel GetAnhLoaiBangCu(int loaiBangId, int donViId);

        void UpdateTruongDuLieuLoaiBang(List<TruongDuLieuLoaiBangViewModel> truongDuLieuLoaiBangs, int donViId);

        void UpdateTruongDuLieuLoaiBangBanSaoTheoBangGoc(int id, int donViId);

        void UpdateTruongDuLieuLoaiBangBanSao(List<TruongDuLieuLoaiBangViewModel> truongDuLieuLoaiBangViewModels, int donViId);

        List<LoaiBangViewModel> GetLoaiBangsChoTruong(int donViId);
    }
}
