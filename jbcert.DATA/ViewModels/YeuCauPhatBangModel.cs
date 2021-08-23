using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class YeuCauPhatBangModel
    {
        public int BangId { get; set; }

        public string Mota { get; set; }

        public DateTime NgayTaoYeuCau { get; set; }

        public int TrangThaiYeuCauPhatBangId { get; set; }

        public int DonViId { get; set; }
        public List<string> LstDuongDanAnh { get; set; }
    }
}
