using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class UpdatePhongBansTrongThongBaoTypeViewModel
    {
        public int? ThongBaoTypeId { get; set; }
        public List<PhongBanViewModel> PhongBans { get; set; }
    }
}
