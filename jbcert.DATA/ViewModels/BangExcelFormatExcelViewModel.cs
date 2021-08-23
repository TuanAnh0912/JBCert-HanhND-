using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class BangExcelFormatExcelViewModel
    {
        public List<ThongTinTruongDuLieuHocSinhExcelViewModel> ThongTinChungs { get; set; }

        public List<HocSinhExcelViewModel> HocSinhs { get; set; }

        public List<BangViewModel> Bangs { get; set; }
    }
}
