using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class LoaiYeuCauViewModel
    {
        public int Id { get; set; }
        public string TenLoaiYeuCau { get; set; }
        public int? MauCongVanId { get; set; }
        public string NoiDung { get; set; }

        public string TenCongVan { get; set; }
        public List<MauCongVanViewModel> MauCongVans { get; set; }
        public List<LoaiBangViewModel> LoaiBangs { get; set; }
    }
}
