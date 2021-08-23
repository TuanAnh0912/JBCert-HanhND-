using jbcert.API.Hubs.Clients;
using jbcert.DATA.Interfaces;
using jbcert.DATA.Provider;
using jbcert.DATA.ViewModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace jbcert.API.Hubs
{
    public class NotificationHub : Hub<INotificationClient>
    {
        NguoiDungProvider _nguoiDungProvider;
        INotification _notificationProvider;
        public NotificationHub()
        {
            _nguoiDungProvider = new NguoiDungProvider();
            _notificationProvider = new NotificationProvider();
        }

        public async Task AddToGroup(string username)
        {
            try
            {
                var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(username);
                UserConnectionViewModel userConnection = new UserConnectionViewModel();
                userConnection.UserId = nguoiDung.NguoiDungId.ToString();

                // get room by phongban
                List<RoomViewModel> roomViewModels = _notificationProvider.GetRoomsByPhongBan(nguoiDung.PhongBan.PhongBanId, nguoiDung.DonViId.Value);


                // get old connection id if exist
                userConnection = _notificationProvider.GetConnectionIdOfUser(userConnection, nguoiDung.DonViId.Value);
                if (userConnection != null && userConnection.ConnectionId != null)
                {
                    NotificationMessage notificationMessage = new NotificationMessage();
                    notificationMessage.Message = "Tài khoản đã được đăng nhập ở một thiết bị khác";
                    notificationMessage.Code = "MUSTLOGOUT";
                    await Clients.Client(userConnection.ConnectionId).ReceiveMessage(notificationMessage);

                    if (roomViewModels != null && roomViewModels.Count > 0)
                    {
                        foreach (string roomName in roomViewModels.Select(x => x.RoomName))
                        {
                            await Groups.RemoveFromGroupAsync(userConnection.ConnectionId, roomName);
                        }
                    }
                }
                // add new conenction Id
                UserConnectionViewModel userConnection_1 = new UserConnectionViewModel();
                userConnection_1.ConnectionId = Context.ConnectionId;
                userConnection_1.UserId = nguoiDung.NguoiDungId.ToString();
                userConnection_1.PhongBanId = nguoiDung.PhongBan.PhongBanId;
                userConnection_1.DonViId = nguoiDung.DonViId.Value;
                _notificationProvider.UpdateConnectionIdOfUser(userConnection_1, nguoiDung.DonViId.Value);

                if (roomViewModels != null && roomViewModels.Count > 0)
                {
                    foreach (string roomName in roomViewModels.Select(x => x.RoomName))
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                    }
                }
            }
            catch(Exception ex)
            {
                NotificationMessage notificationMessage = new NotificationMessage();
                notificationMessage.Message = ex.Message;
                notificationMessage.Code = "ERROR";
                await Clients.Client(Context.ConnectionId).ReceiveMessage(notificationMessage);
            }


        }

        public async Task RemoveFromGroup(string username)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(username);
            UserConnectionViewModel userConnection = new UserConnectionViewModel();
            userConnection.UserId = nguoiDung.NguoiDungId.ToString();

            // get room by phongban
            List<RoomViewModel> roomViewModels = _notificationProvider.GetRoomsByPhongBan(nguoiDung.PhongBan.PhongBanId, nguoiDung.DonViId.Value);


            // get old connection id if exist
            userConnection = _notificationProvider.GetConnectionIdOfUser(userConnection, nguoiDung.DonViId.Value);
            if (userConnection != null)
            {
                if (roomViewModels != null && roomViewModels.Count > 0)
                {
                    foreach (string roomName in roomViewModels.Select(x => x.RoomName))
                    {
                        await Groups.RemoveFromGroupAsync(userConnection.ConnectionId, roomName);
                    }
                }
            }
        }
    }
}
