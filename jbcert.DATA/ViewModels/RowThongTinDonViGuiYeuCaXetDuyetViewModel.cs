using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class RowThongTinDonViGuiYeuCaXetDuyetViewModel
    {

        public int? DonViGuiId { get; set; }

        public string DonViGui { get; set; }

        public int? DonViNhanId { get; set; }

        public string DonViNhan { get; set; }

        public int? LoaiBangId { get; set; }

        public string TenLoaiBang { get; set; }

        public int? CapDonViId { get; set; }
    }
}
