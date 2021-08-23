using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class LogCapPhoi
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PhoiId { get; set; }

        public string SoHieu { get; set; }

        public int? DonViCapId { get; set; }

        public int? DonViNhanId { get; set; }

        public DateTime? NgayCap { get; set; }
    }
}
