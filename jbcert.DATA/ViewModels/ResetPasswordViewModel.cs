using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string ComfirmPassword { get; set; }

        public string Code { get; set; }

        public string CallBackUrl { get; set; }
    }
}
