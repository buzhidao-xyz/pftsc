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

namespace PFTSDesktop.View.Monitoring
{
    /// <summary>
    /// VideoRecordHolderItem.xaml 的交互逻辑
    /// </summary>
    public partial class VideoRecordHolderItem : UserControl
    {
        private view_camera_info m_camera;
        private PFTSHwCtrl.PFTSVideoProxy m_videoProxy;
        private bool m_cacheRecoding = false;
        private string m_cacheRecordingFile = "";

        public VideoRecordHolderItem(view_camera_info camera)
        {
            InitializeComponent();

            int port = 8000;
            if (camera.port != null) port = camera.port.Value;

            m_videoProxy = new PFTSHwCtrl.PFTSVideoProxy(camera.ip, port, camera.admin, camera.password);
            m_videoProxy.Login();
            m_videoProxy.SetWindow(videoControl);

            this.Loaded += VideoRecordHolderItem_Loaded;
        }

        private void VideoRecordHolderItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_cacheRecoding)
            {
                VideoRecordStart(m_cacheRecordingFile);
            }
        }

        public void VideoRecordStart(string filename)
        {
            if (!this.IsLoaded)
            {
                m_cacheRecoding = true;
                m_cacheRecordingFile = filename;
                return;
            }
            if (!m_videoProxy.IsRealView)
            {
                if (!m_videoProxy.StartRealView())
                {
                    MessageBox.Show(m_videoProxy.GetLastError());
                    return;
                }
            }
            if (!m_videoProxy.IsRecording)
            {
                if (!m_videoProxy.StartRecord(filename))
                {
                    MessageBox.Show(m_videoProxy.GetLastError());
                }
            }
        }

        public void VideoRecordStop()
        {
            if (!this.IsLoaded)
            {
                return;
            }
            if (m_videoProxy.IsRecording)
            {
                if (!m_videoProxy.StopRecord())
                {
                    MessageBox.Show(m_videoProxy.GetLastError());
                }
            }
        }
    }
}
