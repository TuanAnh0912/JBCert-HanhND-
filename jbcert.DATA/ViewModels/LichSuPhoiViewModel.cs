using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class LichSuPhoiViewModel
    {
        public int Id { get; set; }

        public int PhoiId { get; set; }

        public string SoHieu { get; set; }

        public int? DonViCapId { get; set; }

        public int? DonViNhanId { get; set; }

        public DateTime? NgayCap { get; set; }

        public string TenDonViCap { get; set; }

        public string TenDonViNhan { get; set; }
    }
}
