using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class AnhLoaiBang
    {
        public string Id { get; set; }
        public int? ObjectId { get; set; }
        public string Url { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhat { get; set; }
        public bool? IsDeleted { get; set; }
        public int DonViId { get; set; }
    }
}
