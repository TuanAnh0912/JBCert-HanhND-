using jbcert.DATA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Interfaces
{
    public interface IPhoi
    {
        void AddPhois(AddPhoiViewModel addPhoiViewModel, int donViId);

        void UpdatePhoi(PhoiViewModel phoiViewModel, int donViId);

        void AddAnhPhoi(List<AttachFileViewModel> attachFileViewModels, int donViId);

        void DeleteAnhPhoi(List<AttachFileViewModel> attachFileViewModels, int donViId);

        PhoiViewModel GetPhoi(int phoiId, int donViId);

        List<AttachFileViewModel> GetAnhPhois(int phoiId, int donViId);

        PhoisWithPaginationViewModel GetPhoisTrongLoaiBang(int loaiBangId, int trangThaiPhoiId, int donViId, int currentPage);

        List<TongSoPhoiTungTrangThaiPhoiTrongLoaiBangViewModel> GetTongSoPhoiTungTrangThaiPhoiTrongLoaiBang(int loaiBangId, int donViId);

        List<SoPhoiNhanTheoTungThangViewModel> GetSoPhoiNhanTheoTungThang(int loaiBangId, int year, int month, int donViId);

        void CapNhatTrangThaiDanhSachPhoiDeIn(int loaiBangId, int soLuongPhoi ,int donViId, Guid nguoiDungId);

        GetThongSoPhoiTheoNamVaLoaiBangViewModel GetThongSoPhoiTheoNamVaLoaiBang (int? loaiBangId, int? nam, int donViId);
    }
}
