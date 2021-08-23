using jbcert.DATA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Interfaces
{
    public interface INotification
    {
        public List<RoomViewModel> GetPhongBansChoThongBaoType(int thongBaoTypeId, int donViId);

        public void UpdatePhongBansTrongThongBaoType(UpdatePhongBansTrongThongBaoTypeViewModel updatePhongBansTrongThongBaoTypeViewModel, int donViId);

        public void UpdateConnectionIdOfUser(UserConnectionViewModel userConnectionViewModel, int donViId);

        public UserConnectionViewModel GetConnectionIdOfUser(UserConnectionViewModel userConnectionViewModel, int donViId);

        public List<RoomViewModel> GetRoomsByPhongBan(int phongBanId, int donViId);

        public void AddThongBao(List<ThongBaoViewModel> thongBaoViewModels,int donViId);

        public ThongBaoWithPaginationViewModel GetThongBaosByPhongBan(int currentPage, int pageSize, int phongBanId, int donViId);

        void UpdateDaDoc(ThongBaoViewModel thongBaoViewModel,int phongBanNhanId, int donViNhanId);

        void UpdateDaDocAll(int phongBanNhanId, int donViNhanId);
    }
}
