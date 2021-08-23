using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class YeuCauPhatBang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int YeuCauPhatBangId { get; set; }

        public int BangId { get; set; }

        public string Mota { get; set; }

        public DateTime NgayTaoYeuCau { get; set; }

        public int TrangThaiYeuCauPhatBangId { get; set; }

        public int DonViId { get; set; }
    }
}
