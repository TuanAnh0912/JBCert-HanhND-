using jbcert.DATA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jbcert.API.Hubs.Clients
{
    public interface INotificationClient
    {
        Task ReceiveMessage(NotificationMessage message);
    }
}
