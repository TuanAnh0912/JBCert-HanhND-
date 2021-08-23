using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class DanhSachSoGocViewModel
    {
        public List<BangViewModel> SoGocs { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPage { get; set; }
    }
}
