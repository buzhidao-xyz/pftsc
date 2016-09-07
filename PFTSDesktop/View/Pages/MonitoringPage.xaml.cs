using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PFTSModel;
using PFTSModel.Services;

namespace PFTSDesktop.View.Pages
{
    /// <summary>
    /// MonitoringPage.xaml 的交互逻辑
    /// </summary>
    public partial class MonitoringPage : Page
    {
        public MonitoringPage()
        {
            InitializeComponent();
            // 监控模式
            controlScene.CameraMode = PFTSScene.CameraMode.Hidden;
            controlScene.RFIDMode = PFTSScene.RFIDMode.Monitoring;
            controlScene.BTrackerRealVideoDelegate += ControlScene_BTrackerRealVideo;
            controlScene.RoomRealVideoDelegate += ControlScene_RoomRealVideoDelegate;

            Global.currentMainWindow.BTrackerMoveDelegete += CurrentMainWindow_BTrackerMoveDelegete;

            var btrackers = new BTrackerService();
            var bs = btrackers.GetAllInscene();
            foreach (var b in bs)
            {
                controlScene.AddAPeople(b);
            }
        }

        private void CurrentMainWindow_BTrackerMoveDelegete(int btrackerId)
        {
            controlScene.RefreshPeople(btrackerId);
            //throw new NotImplementedException();
        }

        private void ControlScene_BTrackerRealVideo(btracker btracker)
        {
            if (btracker.room_id == null) return;
            PFTSModel.dev_camera camera = (new PFTSModel.Services.CameraPositionService()).GetByRoomId(btracker.room_id.Value);
            if (camera == null)
            {
                MessageBox.Show("未检测到该嫌疑人所在房间的摄像头信息");
                return;
            }
            Monitoring.VideoListWindow vw = new Monitoring.VideoListWindow();
            vw.Show();
        }

        private void ControlScene_RoomRealVideoDelegate(view_rfid_room_info room)
        {
            PFTSModel.dev_camera camera = (new PFTSModel.Services.CameraPositionService()).GetByRoomId(room.id);
            if (camera == null)
            {
                MessageBox.Show("未检测到该房间的摄像头信息");
                return;
            }
            Monitoring.VideoListWindow vw = new Monitoring.VideoListWindow();
            vw.Show();
        }
    }
}
