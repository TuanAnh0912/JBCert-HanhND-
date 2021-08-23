using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class HocSinhTrongNhomTaoVanBang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int NhomTaoVanBangId { get; set; }

        public int? HocSinhId { get; set; }

        public int? BangId { get; set; }

        public int? TrangThaiBangId { get; set; }

        public int? BangCuId { get; set; }

        public int? DonViId { get; set; }
    }
}
