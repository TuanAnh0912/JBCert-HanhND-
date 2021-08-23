using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class LogThongTinCaiChinhViewModel
    {
        public List<ChiTietThongTinCaiChinhViewModel> ChiTietThongTinCaiChinhs { get; set; }

        public int LanCaiChinh { get; set; }

        public int CaiChinhId { get; set; }

        public string LyDoCaiChinh { get; set; }

        public int ThongTinCaiChinhId { get; set; }

        public string ThongTinThayDoi { get; set; }

        public string ThongTinCu { get; set; }

        public string ThongTinMoi { get; set; }

        public DateTime? ThoiGianThucHien { get; set; }

    }
}
