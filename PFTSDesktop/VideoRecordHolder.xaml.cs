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
using System.Windows.Shapes;
using PFTSModel;
using PFTSDesktop.View.Monitoring;
using System.IO;

namespace PFTSDesktop
{
    /// <summary>
    /// VideoRecordHolder.xaml 的交互逻辑
    /// </summary>
    public partial class VideoRecordHolder : Window
    {
        private Dictionary<int, view_camera_info> m_mapCameras = new Dictionary<int, view_camera_info>();
        private Dictionary<int, VideoRecordHolderItem> m_mapRecordItems = new Dictionary<int, VideoRecordHolderItem>();

        public VideoRecordHolder()
        {
            InitializeComponent();
        }

        public void CheckDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public void StartRecord(view_camera_info camera)
        {
            var now = System.DateTime.Now;
            var dir = System.Configuration.ConfigurationManager.AppSettings["video_dir"];
            CheckDir(dir);
            var strNow = now.ToString("yyyyMMddHHmmss");
            string filename = string.Format("{0}REC-{1}-{2}.mp4",dir,camera.id,strNow);
            if (m_mapCameras.ContainsKey(camera.id))
            {
                m_mapRecordItems[camera.id].VideoRecordStart(filename);
            }else
            {
                var item = new VideoRecordHolderItem(camera);
                item.Height = 300;
                item.Width = 300;
                m_mapCameras.Add(camera.id, camera);
                item.VideoRecordStart(filename);
                m_mapRecordItems.Add(camera.id, item);
                wrapPanel.Children.Add(item);
            }
        }

        public void StopRecord(view_camera_info camera)
        {
            if (m_mapCameras.ContainsKey(camera.id))
            {
                m_mapRecordItems[camera.id].VideoRecordStop();
            }
        }
    }
}
