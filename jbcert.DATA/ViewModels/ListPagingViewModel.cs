using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class ListPagingViewModel
    {
        public IEnumerable<Object> listOfObj { get; set; }
        public int Total { get; set; }
        public int numberOfPage { get; set; }
    }
}
