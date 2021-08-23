using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbcert.DATA.ViewModels;

namespace jbcert.DATA.ViewModels
{
    public class ThongTinSoGocViewModel
    {
        public List<ThongTinSoGocChungViewModel> Sogocchung { get; set; }
        public List<ThongTinSoGocRiengViewModel> Sogocrieng { get; set; }
    }
}
