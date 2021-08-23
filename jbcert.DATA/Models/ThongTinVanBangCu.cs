using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class ThongTinVanBangCu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TruongDuLieuCode { get; set; }
        public int BangId { get; set; }
        public string GiaTri { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhat { get; set; }
        public int DonViId { get; set; }
        public int? CaiChinhOrder { get; set; }
        public virtual BangCu Bang { get; set; }
    }
}
