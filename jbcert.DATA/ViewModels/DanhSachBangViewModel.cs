using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class DanhSachBangViewModel
    {
        public List<BangViewModel> VanBangs { get; set; }

        public List<TruongDuLieuLoaiBangViewModel> TruongDuLieuLoaiBangs { get; set; } 

        public int? TotalPage { get; set; }

        public int? CurrentPage { get; set; }

        public int? NhomTaoVanBangId { get; set; }

        public int? TrangThaiBangId { get; set; }

        public int? LoaiNhomTaoVanBangId { get; set; }

        public string Title { get; set; }

        public string TruongHoc { get; set; }

        public bool? ChoPhepTaoLai { get; set; }

        public int? DonViIn { get; set; }

        public bool? CanDelete { get; set; }

        public int? IsBanSao { get; set; }

        public bool? IsSoGD { get; set; }
    }
}
