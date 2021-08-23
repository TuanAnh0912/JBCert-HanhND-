using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class AnhYeuCauPhatBang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnhYeuCauPhatBangId { get; set; }

        public string DuongDanAnh { get; set; }

        public int YeuCauPhatBangId { get; set; }
    }
}
