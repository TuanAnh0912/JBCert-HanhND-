using System;
using System.Collections.Generic;
using System.Text;

namespace jbcert.DATA.ViewModels
{
    public class DonViViewModel
    {
        public int DonViId { get; set; }
        public string TenDonVi { get; set; }
        public string TinhId { get; set; }
        public string Tinh { get; set; }

        public string HuyenId { get; set; }
        public string Huyen { get; set; }

        public string XaId { get; set; }

        public string Xa { get; set; }

        public Nullable<int> KhoaChaId { get; set; }
        public Nullable<int> CapDonViId { get; set; }
        public string MaDonVi { get; set; }

        public string DiaGioiHanhChinh { get; set; }

        public string Code { get; set; }

        public string TenDonViCha { get; set; }

        public string TenCapDonVi { get; set; }

        public string LoaiBang { get; set; }

        public int LoaiBangId { get; set; }

        public int? Level { get; set; }

        public List<LoaiBangViewModel> LoaiBangs { get; set; }
    }
}
