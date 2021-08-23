using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class ThongKeChungTheoDonViViewModel
    {
        public int SoBangDaInTrongNam { get; set; }

        public int TongSoHSTotNghiepTrongNam { get; set; }

        public int SoPhoiDaNhanTrongNam { get; set; }

        public int SoLanDinhChinhTrongNam { get; set; }

        public ThongKeYeuCauCongNhanTotNghiepTheoDonViViewModel ThongKeYeuCauCongCongNhanTotNghieps { get; set; }

        public ThongKeYeuCauCapPhoiTheoDonViViewModel ThongKeYeuCauCapPhois { get; set; }

        public List<SoLuongThongKeTungNamViewModel> HocSinhTotNghiepQuaTungNam { get; set; }

        public int Max { get; set; }

        public int Min { get; set; }

        public bool? IsTruong { get; set; }
    }
}
