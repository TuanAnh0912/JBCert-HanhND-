using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class NhomTaoVanBangViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int? TruongHocId { get; set; }

        public string TruongHoc { get; set; }

        public int? DonViId { get; set; }

        public int? LoaiBangId { get; set; }

        public string LoaiBang { get; set; }

        public int TrangThaiBangId { get; set; }

        public string TrangThaiBang { get; set; }

        public int? TongSohocSinh { get; set; }

        public bool? ChoPhepTaoLai { get; set; }

        public int? LoaiNhomTaoVanBangId { get; set; }

        public int? DonViIn { get; set; }

        public bool? IsSoGD { get; set; }

        public int? IsBanSao { get; set; }

        public DateTime NgayTao { get; set; }

        public Guid? NguoiTao { get; set; }

        public DateTime NgayCapNhat { get; set; }

        public Guid? NguoiCapNhat { get; set; }

        public bool? CanDelete { get; set; }
        public bool? AddedByImport { get; set; }

        public List<HocSinhTaoVanBangViewModel> HocSinhs { get; set; }

        public bool IsDeleted { get; set; }

        public string DuongDanFileAnh { get; set; }
        public string DuongDanFileAnhDeIn { get; set; }
    }
}
