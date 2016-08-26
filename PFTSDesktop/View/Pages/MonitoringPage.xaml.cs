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
            controlScene.CameraMode = PFTSScene.CameraMode.Monitoring;
            controlScene.RFIDMode = PFTSScene.RFIDMode.Monitoring;
            controlScene.BTrackerRealVideo += ControlScene_BTrackerRealVideo;

            var btrackers = new BTrackerService();
            var bs = btrackers.GetAllInscene();
            foreach (var b in bs)
            {
                controlScene.AddAPeople(b);
            }
        }

        private void ControlScene_BTrackerRealVideo(btracker btracker)
        {
            Monitoring.VideoListWindow vw = new Monitoring.VideoListWindow();
            vw.Show();
        }
    }
}
