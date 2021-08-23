using jbcert.DATA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class HocSinhExcelViewModel
    {
        public List<ThongTinTruongDuLieuHocSinhExcelViewModel> ThongTinRiengs { get; set; }

        public int TruongHocId { get; set; }

        public HocSinhViewModel TTHS { get; set; } 
    }
}
