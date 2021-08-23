using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class ResultModel
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public string IdReturn { get; set; }
        public ResultModel(bool status, string mess)
        {
            this.Status = status;
            this.Message = mess;
        }
        public ResultModel(bool status, string mess, string id)
        {
            this.Status = status;
            this.Message = mess;
            this.IdReturn = id;
        }
    }
}
