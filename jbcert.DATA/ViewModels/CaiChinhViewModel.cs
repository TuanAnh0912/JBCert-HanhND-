using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class CaiChinhViewModel
    {
        public int CaiChinhId { get; set; }
        public int LanCaiChinh { get; set; }
        public int BangId { get; set; }
        public int BangCuId { get; set; }
        public Guid NguoiThucHien { get; set; }
        public string TenNguoiThucHien { get; set; }
        public string NoiThucHien { get; set; }
        public int DonViThucHien { get; set; }
        public string TenDonViThucHien { get; set; }
        public DateTime ThoiGianThucHien { get; set; }
        public string LyDoCaiChinh { get; set; }
        public List<ThongTinCaiChinhVewModel> ThongTinCaiChinhs { get; set; }
        public List<FileDinhKemCaiChinhViewModel> Files { get; set; }
    }
}
