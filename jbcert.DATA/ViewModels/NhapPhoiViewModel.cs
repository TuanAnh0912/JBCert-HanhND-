using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class NhapPhoiViewModel
    {
        public int NhapPhoiId { get; set; }
        public string Ma { get; set; }
        public DateTime NgayTao { get; set; }
        public Guid NguoiTao { get; set; }
        public string TenNguoiTao { get; set; }
        public int SoLuong { get; set; }
        public int SoLuongDaNhap { get; set; }
        public string DiaDiemNhan { get; set; }
        public int? LoaiBangId { get; set; }
        public string TenLoaiBang { get; set; }
        public string SoDienThoaiNguoiTao { get; set; }
        public int DonViId { get; set; }
        public int? IsBanSao { get; set; }
        public List<PhoiVanBangViewModel> Phois { get; set; }
        
        public PaginationViewModel Pagination { get; set; }
    }
}
