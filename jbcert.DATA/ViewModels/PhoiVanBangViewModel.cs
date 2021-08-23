using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class PhoiVanBangViewModel
    {
        public int Id { get; set; }
        public int? LoaiBangId { get; set; }
        public string TenLoaiBang { get; set; }
        public string SoHieu { get; set; }
        public string ChiSoCoDinh { get; set; }
        public string ChiSoThayDoi { get; set; }
        public int? TrangThaiPhoiId { get; set; }
        public string TenTrangThai { get; set; }
        public string MaMau { get; set; }
        public string Border { get; set; }
        public string MauChu { get; set; }
        public string MoTaTrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public string TenNguoiNhap { get; set; }
        public bool? IsDeleted { get; set; }
        public int? DonViId { get; set; }
        public int? NhapPhoiId { get; set; }
        public bool? DaCap { get; set; }
        public int? ChinhSua { get; set; }

        public int? IsBanSao { get; set; }

        public List<PhoiFileDinhKemViewModel> FileDinhKemPhois { get; set; }
    }
}
