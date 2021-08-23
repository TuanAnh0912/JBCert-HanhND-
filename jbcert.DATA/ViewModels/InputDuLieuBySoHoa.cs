using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class InputDuLieuBySoHoa
    {
        public int loaiBangId { get; set; }
        public List<IDictionary<string, string>> lstData { get; set; }
    }
}
