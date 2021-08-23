using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class LogCaiChinhViewModel
    {
        public int LogId { get; set; }
        public int? BangId { get; set; }
        public string TruongDuLieuCode { get; set; }
        public string GiaTri { get; set; }
        public string HanhDong { get; set; }
        public DateTime? ThoiGian { get; set; }
        public Guid? NguoiDungId { get; set; }
        public string HoTen { get; set; }
        public string Ip { get; set; }

    }
}
