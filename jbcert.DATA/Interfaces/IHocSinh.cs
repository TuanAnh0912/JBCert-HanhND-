using jbcert.DATA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Interfaces
{
    public interface IHocSinh
    {
        public void ImportSoGoc(List<HocSinhViewModel> hocSinhViewModels, int donViId);

        HocSinhsWithPaginationViewModel GetHocSinhs(int lopHocId, int namTotNghiep,int donViId);

        HocSinhsWithPaginationViewModel GetDanhSachCanPheDuyet(int lopHocId, int namTotNghiep, int donViId);

        HocSinhViewModel GetHocSinh(int hocSinhId, int donViId);

        int GetSTTHocSinhDuocXet(int namTotNghiep, int donViId);

        HocSinhsWithPaginationViewModel GetDanhSachDaXetDuyet(string hoVaTen, int? namSinh, int? gioiTinhId,int? lopHocId, string nienKhoa, int? danTocId, string hL, string hK, bool? kQ, string xepLoaiTotNghiep, bool? congNhanTotNghiep, int currentPage, int donViId);

        public HocSinhsWithPaginationViewModel GetDanhSachDuXetCongNhanTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? lopHocId, string nienKhoa, int? danTocId, string hL, string hK, bool? kQ, string xepLoaiTotNghiep, bool? congNhanTotNghiep, int currentPage, int donViId);

        HocSinhsWithPaginationViewModel GetDanhSachDaCongNhanTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, int currentPage, int donViId);

        HocSinhsWithPaginationViewModel GetDanhSachHocSinhDuXetCapTruong(string hoVaTen, int? namSinh, int? gioiTinhId, int? lopHocId, int? namTotNghiep, int? danTocId, string hL, string hK, bool? kQ, string xepLoaiTotNghiep, bool? congNhanTotNghiep, int donViId);

        HocSinhsWithPaginationViewModel GetDanhSachDaTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, bool? daInBangGoc, int currentPage, int donViId);
        
        void AddHocSinh(HocSinhViewModel hocSinhViewModel, int donViId, string ip);

        void UpdateHocSinh(HocSinhViewModel hocSinhViewModel, int donViId, string ip);

        void UpdateTrangThaiXetDuyet(HocSinhViewModel hocSinhViewModel, int donViId, string ip);

        void DeleteHocSinhs(List<HocSinhViewModel> hocSinhViewModels, int donViId, string ip);

        public List<MonHocViewModel> ListMonHoc(int donViId);

        public List<NienKhoaViewModel> GetNienKhoas(int donViId);

        HocSinhsWithPaginationViewModel ExportDanhSachDaXetDuyetTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? lopHocId, int? namTotNghiep, int? danTocId, string hL, string hK, bool? kQ, string xepLoaiTotNghiep, bool? congNhanTotNghiep, int donViId);
        
        HocSinhsWithPaginationViewModel ExportDanhSachDaCongNhanTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? truongHocId, int? namTotNghiep, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, int donViId);

        HocSinhsWithPaginationViewModel ExportDanhSachDaTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, bool? daInBangGoc, int donViId);

        List<LopHocViewModel> GetLopHocByGiaoVien(Guid nguoiDungId, int donViId);

        List<LopHocViewModel> GetAllLopHocTrongTruong(int donViId);

        //void AddMonHoc(MonHocViewModel monHocViewModel, int donViId);

        //void UpdateMonHoc(MonHocViewModel monHocViewModel, int donViId);

        //void DeleteMonHoc(MonHocViewModel monHocViewModel, int donViId);

        LopHocViewModel GetLopHoc(int lopHocId, int donViId);

        void AddLopHoc(LopHocViewModel lopHocViewModel, int donViId);

        void UpdateLopHoc(LopHocViewModel lopHocViewModel, int donViId);

        void DeleteLopHoc(int lopHocId);

        public List<MonHocViewModel> GetMonHocsByCapDonVi(string codeCapDonVi);

        public void UpdateSoVaoSo(List<HocSinhViewModel> hocSinhViewModels, int donViId);

        public void CongNhanTotNghiepHocSinhTrongYeuCau(int yeuCauId, string maTruongHoc, string maDonViSo, int donViId);

        HocSinhsWithPaginationViewModel GetDanhSachDaCongNhanTotNghiepTheoDonVi(string hoVaTen, int? namSinh, int? lopHocId, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, int currentPage, int donViId);

        HocSinhsWithPaginationViewModel ExportDanhSachDaCongNhanTotNghiepTheoDonVi(string hoVaTen, int? namSinh, int? lopHocId, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, int donViId);

    }
}
