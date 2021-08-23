using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class ThongTinCaiChinh
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ThongTinCaiChinhId { get; set; }
        public int CaiChinhId { get; set; }
        public string ThongTinThayDoi { get; set; }
        public string ThongTinCu { get; set; }
        public string ThongTinMoi { get; set; }
    }
}
