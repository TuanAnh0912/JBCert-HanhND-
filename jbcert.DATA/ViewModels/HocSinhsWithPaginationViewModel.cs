using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class HocSinhsWithPaginationViewModel
    {
        public List<HocSinhViewModel> HocSinhs { get; set; }

        public List<MonHocViewModel> MonHocs { get; set; }

        public string HoiDong { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPage { get; set; }

        public string TenTruong { get; set; }

        public string TenHuyen { get; set; }
        public string CapTruong { get; set; }

        public int Ngay { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }

        public int TongSoHocSinhCongNhanTotNghiep { get; set; }

        public string NienKhoa { get; set; }
        public int TongSoHocSinhLoaiGioi { get; set; }
        public int TongSoHocSinhLoaiKha{ get; set; }
        public int TongSoHocSinhLoaiTrungBinh { get; set; }
        public int TongSoHocSinhNam { get; set; }
        public int TongSoHocSinhNu { get; set; }
        public int TongSoHocSinhUT { get; set; }
        public int TongSoHocSinhKK { get; set; }
        public bool? IsTruong { get; set; }
    }
}
