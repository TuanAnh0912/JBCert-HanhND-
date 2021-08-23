using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class GetThongSoPhoiTheoNamVaLoaiBangViewModel
    {
        public int? LoaiBangId { get; set; }

        public int? Nam { get; set; } 

        public int TongSoPhoiDaNhap { get; set; }

        public int TongSoPhoiDaIn { get; set; }

        public int TongSoPhoiConLai { get; set; }

        public int TongSoPhoiLoi { get; set; }
    }
}
