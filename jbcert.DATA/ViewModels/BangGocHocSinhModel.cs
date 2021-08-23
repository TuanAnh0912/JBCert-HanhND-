using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class BangGocHocSinhModel
    {
        public int HocSinhId { get; set; }
        public List<String> Code { get; set; }
        public List<String> GiaTri { get; set; }
        public bool Checked { get; set; }
    }
}
