using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class ThongTinVBViewModel
    {
        public int Id { get; set; }
        public string TruongDuLieuCode { get; set; }
        public string TenTruongDuLieu { get; set; }
        public int KieuDuLieu { get; set; }
        public int BangId { get; set; }
        public string GiaTri { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhat { get; set; }
        public int DonViId { get; set; }

        public int? CanUpdate { get; set; }
    }
}
