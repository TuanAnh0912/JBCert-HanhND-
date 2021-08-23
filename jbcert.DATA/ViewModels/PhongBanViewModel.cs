using jbcert.DATA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class PhongBanViewModel
    {
        public int PhongBanId { get; set; }

        public string TenPhongBan { get; set; }

        public string Mota { get; set; }

        public List<NguoiDungViewModel> nguoiDungs { get; set; } 

        public Nullable<int> DonViId { get; set; }
        public string TenDonVi { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }
}
