using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class NhomSoHoaWithPaginationViewModel
    {
        public List<SoHoaViewModel> SoHoas { get; set; }

        public int TotalPage { get; set; }
    }
}
