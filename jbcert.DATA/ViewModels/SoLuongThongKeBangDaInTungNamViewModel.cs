using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class SoLuongThongKeBangDaInTungNamViewModel
    {
        public List<SoLuongThongKeTungNamViewModel> BangGocs { get; set; }
        public List<SoLuongThongKeTungNamViewModel> BanSaos { get; set; }
        public List<SoLuongThongKeTungNamViewModel> CaiChinhs { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int TongSoBangGocDaIn { get; set; }
        public int TongSoBanSaoDaIn { get; set; }
        public int TongSoLanCaiChinh { get; set; }

        public int TongSoSoHoa { get; set; }
    }
}
