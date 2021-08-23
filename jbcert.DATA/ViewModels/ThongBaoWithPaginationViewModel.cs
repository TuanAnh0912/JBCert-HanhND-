using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class ThongBaoWithPaginationViewModel
    {
        public List<ThongBaoViewModel> ThongBaos { get; set; }

        public bool CanLoadMore { get; set; }

        public int SoLuongChuaDoc { get; set; }

        public int CurrentPage { get; set; }

        public int Total { get; set; }

        public int PageSize { get; set; }
    }
}
