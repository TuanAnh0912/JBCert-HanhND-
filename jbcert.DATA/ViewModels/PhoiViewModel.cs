using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class PhoiViewModel
    {
        public int Id { get; set; }
        
        public int LoaiBangId { get; set; }
        
        public string SoHieu { get; set; }

        public string SoVaoSo { get; set; }
        
        public int TrangThaiPhoiId { get; set; }

        public string TrangThaiPhoi { get; set; }
        
        public string MoTa { get; set; }
        
        public string TenNguoiLayPhoi { get; set; }
        
        public string DiaDiemLayPhoi { get; set; }
        
        public DateTime ThoiGianLayPhoi { get; set; }
        
        public DateTime NgayTao { get; set; }
        
        public Guid NguoiTao { get; set; }
        
        public DateTime NgayCapNhat { get; set; }
        
        public Guid NguoiCapNhat { get; set; }
        
        public bool IsDeleted { get; set; }
        
        public int DonViId { get; set; }

        public List<AttachFileViewModel> AnhPhois { get; set; }
    }
}
