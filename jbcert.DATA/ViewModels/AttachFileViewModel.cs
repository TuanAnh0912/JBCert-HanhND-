using System;
using System.Collections.Generic;
using System.Text;

namespace jbcert.DATA.ViewModels
{
    public class AttachFileViewModel
    {
        public string Id { get; set; }

        public string Base64String { get; set; }

        public string Ten { get; set; }

        public string Url { get; set; }

        public int ObjectId { get; set; }

        public DateTime NgayTao { get; set; }

        public Guid NguoiTao { get; set; }

        public DateTime NgayCapNhat { get; set; }

        public Guid NguoiCapNhat { get; set; }

        public int DonViId { get; set; }

        public string Extension { get; set; }

        public bool IsDeleted { get; set; }
    }
}
