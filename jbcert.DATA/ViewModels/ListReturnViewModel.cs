using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class ListReturnViewModel
    {
        public IEnumerable<Object> Data { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}
