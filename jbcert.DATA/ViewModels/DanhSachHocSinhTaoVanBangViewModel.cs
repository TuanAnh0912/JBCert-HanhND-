using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class DanhSachHocSinhTaoVanBangViewModel
    {
        public int YeuCauId { get; set; }

        public int LoaiBangId { get; set; }
        public List<BangViewModel> HocSinhs { get; set; }

        public int CurrentPage { get; set; }

        public int TrangThaiBangId { get; set; }

        public int TotalPage { get; set; }

        public int TotalRow { get; set; }

        public string Title { get; set; }
        public string TruongHoc { get; set; }
        public bool? CanDelete { get; set; }

        public List<TruongDuLieuTrongBangViewModel> TruongDungChungs { get; set; }

        public List<TruongDuLieuLoaiBangViewModel> TruongKhongDungChungs { get; set; }
    }
}
