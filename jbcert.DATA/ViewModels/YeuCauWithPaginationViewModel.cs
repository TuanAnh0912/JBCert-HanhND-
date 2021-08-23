using jbcert.DATA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class YeuCauWithPaginationViewModel
    {
        public List<YeuCauViewModel> YeuCaus { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPage { get; set; }
    }
}
