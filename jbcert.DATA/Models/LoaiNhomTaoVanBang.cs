using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class LoaiNhomTaoVanBang
    {
        [Key]
        public int Id { get; set; }
    
        public string Ten { get; set; }
    }
}
