using jbcert.DATA.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace jbcert.DATA
{
    public class ApplicationDbContext
    {
        JBC_CoreContext db = new JBC_CoreContext();
        public JBC_CoreContext DbContext { get { return db; } }
    }
}
