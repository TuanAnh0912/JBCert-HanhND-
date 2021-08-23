using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class CaiChinhVanBangViewModel
    {
        public Guid NguoiThucHien { get; set; }
        public int DonViId { get; set; }
        public CaiChinhViewModel CaiChinh { get; set; }
        public BangViewModel VanBang { get; set; }
    }
}
