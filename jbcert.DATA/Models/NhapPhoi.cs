using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class NhapPhoi
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NhapPhoiId { get; set; }
        public string Ma { get; set; }
        public DateTime NgayTao { get; set; }
        public Guid NguoiTao { get; set; }
        public int SoLuong { get; set; }
        public string DiaDiemNhan { get; set; }
        public int? LoaiBangId { get; set; }
        public string SoDienThoaiNguoiTao { get; set; }
        public int DonViId { get; set; }
    }
}
