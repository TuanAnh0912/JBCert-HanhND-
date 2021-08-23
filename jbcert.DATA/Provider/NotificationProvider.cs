using jbcert.DATA.Helpers;
using jbcert.DATA.Interfaces;
using jbcert.DATA.Models;
using jbcert.DATA.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Provider
{
    public class NotificationProvider : ApplicationDbContext, INotification
    {
        public List<RoomViewModel> GetRoomsByPhongBan(int phongBanId, int donViId)
        {
            try
            {
                List<RoomViewModel> roomViewModels = new List<RoomViewModel>();

                string sqlString = @"Select * From Rooms
                                    Where (PhongBanId = @PhongBanId) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@PhongBanId", phongBanId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            roomViewModels = MapDataHelper<RoomViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                return roomViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RoomViewModel> GetPhongBansChoThongBaoType(int thongBaoTypeId, int donViId)
        {
            try
            {
                List<RoomViewModel> phongBanViewModels = new List<RoomViewModel>();

                string sqlString = @"Select * From Rooms 
                                    Where (ThongBaoTypeId = @ThongBaoTypeId) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@ThongBaoTypeId", thongBaoTypeId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            phongBanViewModels = MapDataHelper<RoomViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                return phongBanViewModels;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateConnectionIdOfUser(UserConnectionViewModel userConnectionViewModel, int donViId)
        {
            try
            {
                string sqlString = @"Delete UserConnections
                                     Where (UserId = @UserId) and (PhongBanId = @PhongBanId) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@PhongBanId", userConnectionViewModel.PhongBanId));
                        command.Parameters.Add(new SqlParameter("@UserId", userConnectionViewModel.UserId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                UserConnection userConnection = new UserConnection();
                userConnection.DonViId = donViId;
                userConnection.ConnectionId = userConnectionViewModel.ConnectionId;
                userConnection.UserId = userConnectionViewModel.UserId;
                userConnection.PhongBanId = userConnectionViewModel.PhongBanId;
                DbContext.UserConnections.Add(userConnection);
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdatePhongBansTrongThongBaoType(UpdatePhongBansTrongThongBaoTypeViewModel updateThongBaoTypesChoPhongBanViewModel, int donViId)
        {
            try
            {
                // xoa lien ket cu
                string sqlString = @"Delete Rooms 
                                    Where(ThongBaoTypeId = @ThongBaoTypeId) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@ThongBaoTypeId", updateThongBaoTypesChoPhongBanViewModel.ThongBaoTypeId.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                // add lien ket moi
                if (updateThongBaoTypesChoPhongBanViewModel.PhongBans != null  && updateThongBaoTypesChoPhongBanViewModel.PhongBans.Count > 0)
                {
                    List<Room> rooms = new List<Room>();
                    string roomName = donViId + "-" + Guid.NewGuid();
                    foreach (int phongBanId in updateThongBaoTypesChoPhongBanViewModel.PhongBans.Select(x => x.PhongBanId))
                    {
                        Room room = new Room();
                        room.RoomName = roomName;
                        room.DonViId = donViId;
                        room.PhongBanId = phongBanId;
                        room.ThongBaoTypeId = updateThongBaoTypesChoPhongBanViewModel.ThongBaoTypeId;
                        rooms.Add(room);
                    }

                    DbContext.Rooms.AddRange(rooms);
                    DbContext.SaveChanges();
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserConnectionViewModel GetConnectionIdOfUser(UserConnectionViewModel userConnectionViewModel, int donViId)
        {
            try
            {
                string sqlString = @"Select * From UserConnections Where (UserId = @UserId) and (DonViId = @DonViId)";
                UserConnectionViewModel userConnectionViewModel_1 = new UserConnectionViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@UserId", userConnectionViewModel.UserId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            userConnectionViewModel_1 = MapDataHelper<UserConnectionViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                return userConnectionViewModel_1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddThongBao(List<ThongBaoViewModel> thongBaoViewModels, int donViId)
        {
            try
            {
                List<ThongBao> thongBaos = new List<ThongBao>();
                foreach (ThongBaoViewModel thongBaoViewModel in thongBaoViewModels)
                {
                    ThongBao thongBao = new ThongBao();
                    thongBao.Id = thongBaoViewModel.Id;
                    thongBao.NoiDung = thongBaoViewModel.NoiDung;
                    thongBao.Title = thongBaoViewModel.Title;
                    thongBao.NguoiGuiId = thongBaoViewModel.NguoiGuiId;
                    thongBao.ThongBaoTypeId = thongBaoViewModel.ThongBaoTypeId;
                    thongBao.PhongBanGuiId = thongBaoViewModel.PhongBanGuiId;
                    thongBao.DonViGuiId = thongBaoViewModel.DonViGuiId;
                    thongBao.NguoiNhanId = thongBaoViewModel.NguoiNhanId;
                    thongBao.PhongBanNhanId = thongBaoViewModel.PhongBanNhanId;
                    thongBao.DonViNhanId = thongBaoViewModel.DonViNhanId;
                    thongBao.DaDoc = thongBaoViewModel.DaDoc;
                    thongBao.NgayTao = thongBaoViewModel.NgayTao;
                    thongBao.Code = thongBaoViewModel.Code;
                    thongBao.Url = thongBaoViewModel.Url;
                    thongBaos.Add(thongBao);
                }
                DbContext.ThongBaos.AddRange(thongBaos);
                DbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public ThongBaoWithPaginationViewModel GetThongBaosByPhongBan(int currentPage, int pageSize, int phongBanId, int donViId)
        {
            try
            {
                ThongBaoWithPaginationViewModel thongBaoWithPaginationViewModel = new ThongBaoWithPaginationViewModel();
                // get list thong baos
                string sqlString = @"Select a.*, b.Icon From ThongBaos as a
                                    Left Join ThongBaoTypes as  b
                                    on a.ThongBaoTypeId = b.Id
                                    Where (PhongBanNhanId = @PhongBanNhanId) and (DonViNhanId = @DonViNhanId)
                                     Order By NgayTao DESC
                                    Offset @Offset Rows Fetch Next @Next Rows Only";
                UserConnectionViewModel userConnectionViewModel_1 = new UserConnectionViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@PhongBanNhanId", phongBanId));
                        command.Parameters.Add(new SqlParameter("@DonViNhanId", donViId));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * pageSize));
                        command.Parameters.Add(new SqlParameter("@Next", pageSize));
                        using (var reader = command.ExecuteReader())
                        {
                            thongBaoWithPaginationViewModel.ThongBaos = MapDataHelper<ThongBaoViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                // get tong chua doc
                string sqlString_2 = @"Select Count(*) as 'TotalRow' From ThongBaos 
                                    Where (PhongBanNhanId = @PhongBanNhanId) and (DonViNhanId = @DonViNhanId) and (DaDoc = 0)";
                int totalRow_2 = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@PhongBanNhanId", phongBanId));
                        command.Parameters.Add(new SqlParameter("@DonViNhanId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                totalRow_2 = Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                // check loadmore
                string sqlString_1 = @"Select * From ThongBaos Where (PhongBanNhanId = @PhongBanNhanId) and (DonViNhanId = @DonViNhanid)
                                     Order By NgayTao DESC
                                    Offset @Offset Rows Fetch Next @Next Rows Only";
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@PhongBanNhanId", phongBanId));
                        command.Parameters.Add(new SqlParameter("@DonViNhanid", donViId));
                        command.Parameters.Add(new SqlParameter("@Offset", currentPage * pageSize));
                        command.Parameters.Add(new SqlParameter("@Next", pageSize));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                totalRow += 1;
                            }
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                // get total
                string sqlString_3 = @"Select Count(*) as 'TotalRow' From ThongBaos Where (PhongBanNhanId = @PhongBanNhanId) and (DonViNhanId = @DonViNhanid)";
                int totalRow_3 = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_3;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@PhongBanNhanId", phongBanId));
                        command.Parameters.Add(new SqlParameter("@DonViNhanid", donViId));
                        command.Parameters.Add(new SqlParameter("@Offset", currentPage * pageSize));
                        command.Parameters.Add(new SqlParameter("@Next", pageSize));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                totalRow_3 = Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }


                thongBaoWithPaginationViewModel.Total = totalRow_3;
                thongBaoWithPaginationViewModel.SoLuongChuaDoc = totalRow_2;
                thongBaoWithPaginationViewModel.CanLoadMore = totalRow > 0;
                thongBaoWithPaginationViewModel.CurrentPage = currentPage;
                thongBaoWithPaginationViewModel.PageSize = pageSize;
                return thongBaoWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateDaDoc(ThongBaoViewModel thongBaoViewModel, int phongBanNhanId, int donViNhanId)
        {
            try
            {
                // check loadmore
                string sqlString = @"Update ThongBaos
                                    Set DaDoc = 1
                                    Where (Id = @Id) and (DonViNhanId = @DonViNhanId) and (PhongBanNhanId = @PhongBanNhanId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Id", thongBaoViewModel.Id));
                        command.Parameters.Add(new SqlParameter("@PhongBanNhanId", phongBanNhanId));
                        command.Parameters.Add(new SqlParameter("@DonViNhanid", donViNhanId));
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateDaDocAll(int phongBanNhanId, int donViNhanId)
        {
            try
            {
                // check loadmore
                string sqlString = @"Update ThongBaos
                                    Set DaDoc = 1
                                    Where (DonViNhanId = @DonViNhanId) and (PhongBanNhanId = @PhongBanNhanId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@PhongBanNhanId", phongBanNhanId));
                        command.Parameters.Add(new SqlParameter("@DonViNhanid", donViNhanId));
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
