using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public partial class CaiChinh
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CaiChinhId { get; set; }
        public int LanCaiChinh { get; set; }
        public int BangId { get; set; }
        public int BangCuId { get; set; }
        public Guid NguoiThucHien { get; set; }
        public string NoiThucHien { get; set; }
        public int DonViThucHien { get; set; }
        public DateTime ThoiGianThucHien { get; set; }
        public string LyDoCaiChinh { get; set; }
    }
}
