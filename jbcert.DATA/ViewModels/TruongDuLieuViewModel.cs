using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class TruongDuLieuViewModel
    {
        public string? Code { get; set; }
        public string? Ten { get; set; }
        public int KieuDuLieu { set; get; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhat { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsChungChi { get; set; }
        public int? DonViId { get; set; }
    }
}
