using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jbcert.API.Service
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
