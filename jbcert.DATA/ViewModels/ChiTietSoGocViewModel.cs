using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class ChiTietSoGocViewModel
    {
        public BangViewModel SoGoc { get; set; }

        public List<LogVanBangViewModel> LogVanBangs { get; set; }
    }
}
