using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class SoHoa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime? NgayTao { get; set; }

        public int? NamTotNghiep { get; set; }

        public int? LoaiBangId { get; set; }

        public string TenLoaiBang { get; set; }

        public int? DonViId { get; set; }

        public string FileUrl { get; set; }
    }
}
