using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class PaginationViewModel
    {
        public Object Data { get; set; }
        public int? TotalPage { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public int? Count { get; set; }
    }
}
