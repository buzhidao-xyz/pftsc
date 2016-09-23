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
using GFramework.BlankWindow;
using PFTSUITemplate.Element;

namespace PFTSDesktop
{
    /// <summary>
    /// VideoRecordHolder.xaml 的交互逻辑
    /// </summary>
    public partial class VideoRecordHolder : BlankWindow
    {
        private Dictionary<int, view_camera_info> m_mapCameras = new Dictionary<int, view_camera_info>();
        private Dictionary<int, VideoRecordHolderItem> m_mapRecordItems = new Dictionary<int, VideoRecordHolderItem>();

        public VideoRecordHolder()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 双击顶部时放大缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            if (this.ResizeMode == ResizeMode.NoResize) return;

            //判断点击的控件不是Grid类型，就可以双击
            if (!(e.OriginalSource is System.Windows.Controls.Image))
            {
                return;
            }

            if (this.WindowState == WindowState.Normal)
            {
                this.btnNomalMax.Visibility = Visibility.Hidden;
                this.btnMax.Visibility = Visibility.Visible;
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                this.btnNomalMax.Visibility = Visibility.Visible;
                this.btnMax.Visibility = Visibility.Hidden;
            }

        }
        /// <summary>
        /// 窗口关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageWindow.Show("关闭窗口，将退出系统，请确认？", "确认退出") == MessageWindowResult.OK)
            {
                if (Global.currentMainWindow != null)
                {
                    Global.currentMainWindow.StopRecord(delegate() {
                        System.Environment.Exit(0);
                        Global.currentMainWindow.Close();
                    });
                }else
                {
                    System.Environment.Exit(0);
                    Global.currentMainWindow.Close();
                }
            }
//            this.Close();
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSysMax_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                this.btnNomalMax.Visibility = Visibility.Hidden;
                this.btnMax.Visibility = Visibility.Visible;
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.btnNomalMax.Visibility = Visibility.Visible;
                this.btnMax.Visibility = Visibility.Hidden;
            }
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

        public video ReRecord(view_camera_info camera)
        {
            var now = System.DateTime.Now;
            var dir = System.Configuration.ConfigurationManager.AppSettings["video_dir"];
            CheckDir(dir);
            var strNow = now.ToString("yyyyMMddHHmmss");
            string filename = string.Format("{0}REC-{1}-{2}.mp4", dir, camera.id, strNow);
            if (m_mapCameras.ContainsKey(camera.id))
            {
                var item = m_mapRecordItems[camera.id];
                video v = new video();
                var br = item.IsRecording;
                v.start_time = item.RecordTime;
                v.filename = item.Filename;
                item.VideoRecordStop();
                item.VideoRecordStart(filename);
                if (br)
                {
                    v.camera_id = camera.id;
                    v.position_id = camera.position_id == null ? 0 : camera.position_id.Value;
                    v.end_time = DateTime.Now;
                    return v;
                }
            }
            else
            {
                var item = new VideoRecordHolderItem(camera);
                item.Height = 300;
                item.Width = 300;
                m_mapCameras.Add(camera.id, camera);
                item.VideoRecordStart(filename);
                m_mapRecordItems.Add(camera.id, item);
                wrapPanel.Children.Add(item);
            }
            return null;
        }

        public video StopRecord(view_camera_info camera)
        {
            if (m_mapCameras.ContainsKey(camera.id))
            {
                var item = m_mapRecordItems[camera.id];
                video v = new video();
                var br = item.IsRecording;
                v.start_time = item.RecordTime;
                v.filename = item.Filename;
                item.VideoRecordStop();
                if (br)
                {
                    v.camera_id = camera.id;
                    v.position_id = camera.position_id == null ? 0 : camera.position_id.Value;
                    v.end_time = DateTime.Now;
                    return v;
                }
            }
            return null;
        }
    }
}
