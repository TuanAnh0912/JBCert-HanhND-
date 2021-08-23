using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class NhomNguoiDungViewModel
    {
        public int? NhomNguoiDungId { get; set; }
        public string TenNhomNguoiDung { get; set; }
        public Nullable<int> PhongBanId { get; set; }
        public string TenPhongBan { get; set; }
        public List<ChucNangViewModel> ChucNangs { get; set; }
    }
}
