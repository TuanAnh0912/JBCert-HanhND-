using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class DetailThongTinBangViewModel
    {
        public int BangId { get; set; }
        public string HoVaTen { get; set; }

        public string TruongHoc { get; set; }

        public string NamTotNghiep { get; set; }

        public int LoaiBangId { get; set; }

        public string TenLoaiBang { get; set; }

        public List<ThongTinVBViewModel> ThongTinVanBangs { get; set; }
    }
}
