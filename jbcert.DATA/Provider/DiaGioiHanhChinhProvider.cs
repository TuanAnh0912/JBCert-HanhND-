using jbcert.DATA.Helpers;
using jbcert.DATA.Interfaces;
using jbcert.DATA.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace jbcert.DATA.Provider
{
    public class DiaGioiHanhChinhProvider : ApplicationDbContext, IDiaGioiHanhChinh
    {
        public List<HuyenViewModel> GetHuyens(string id, string name)
        {
            try
            {
                List<HuyenViewModel> huyenViewModels = new List<HuyenViewModel>();
                string sqlString = @"Select a.*, b.Ten as 'Tinh' From [dbo].[Huyen] as a
                                    Left Join [dbo].[Tinh] as b
                                    on b.Id = SUBSTRING(a.Id, 1, 2) Where a.Ten Like N'%'+ @Name +'%' and a.Id like @Id + '%'";

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Name", name));
                        command.Parameters.Add(new SqlParameter("@Id", id));
                        using (var reader = command.ExecuteReader())
                        {
                            huyenViewModels = MapDataHelper<HuyenViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return huyenViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HuyenViewModel GetHuyenByXa(string id)
        {
            try
            {
                HuyenViewModel huyenViewModel = new HuyenViewModel();
                string sqlString = @"Select a.*, b.Ten as 'Tinh' From [dbo].[Huyen] as a
                                    Left Join [dbo].[Tinh] as b
                                    on b.Id = SUBSTRING(a.Id, 1, 2) Where a.Id like @Id";

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Id", id));
                        using (var reader = command.ExecuteReader())
                        {
                            huyenViewModel = MapDataHelper<HuyenViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return huyenViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TinhViewModel> GetTinhs(string name)
        {
            try
            {
                List<TinhViewModel> tinhViewModels = new List<TinhViewModel>();
                string sqlString = @"Select * From [dbo].[Tinh] Where Ten Like N'%'+@Name +'%'";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Name", name));
                        using (var reader = command.ExecuteReader())
                        {
                            tinhViewModels = MapDataHelper<TinhViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return tinhViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<XaViewModel> GetXas(string id, string name)
        {
            try
            {
                List<XaViewModel> xaViewModels = new List<XaViewModel>();
                string sqlString = @"Select a.*, b.Ten as 'Huyen', c.Ten as 'Tinh' From [dbo].[Xa] as a
                                    Left Join [dbo].[Huyen] as b
                                    on b.Id = SUBSTRING(a.Id, 1, 4)
                                    Left Join [dbo].[Tinh] as c
                                    on c.Id = SUBSTRING(a.Id, 1, 2)
                                    Where a.Ten Like N'%'+ @Name +'%' and a.Id like @Id + '%'";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Name", name));
                        command.Parameters.Add(new SqlParameter("@Id", id));
                        using (var reader = command.ExecuteReader())
                        {
                            xaViewModels = MapDataHelper<XaViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }


                return xaViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TinhViewModel GetTinhById(string id)
        {
            try
            {
                TinhViewModel tinhViewModel = new TinhViewModel();
                string sqlString = @"Select * From Tinh Where Id = @Id";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Id", id));
                        using (var reader = command.ExecuteReader())
                        {
                            tinhViewModel = MapDataHelper<TinhViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }


                return tinhViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
