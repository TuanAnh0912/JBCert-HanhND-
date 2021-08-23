using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class SearchedCertsWithPaginationViewModel
    {
        public List<SearchedCertsViewModel> SearchedCerts {get;set;}

        public int TotalPage { get; set; }

        public int Total { get; set; }

        public int PageNum { get; set; }
    }
}
