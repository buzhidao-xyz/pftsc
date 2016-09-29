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
using PFTSUITemplate.Controls;
using System.Windows.Threading;
using System.Threading;

namespace PFTSDesktop
{
    #region handler
    // 点击实时画面
    public delegate void BTrackerMoveHandler(int btrackerId);
    #endregion


    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowTemplet
    {
        private PFTSHwCtrl.PFTSRFIDServer m_rfidServer;
        private PFTSHwCtrl.PFTSVideoRecordProxy m_videoRecorder;
        private VideoRecordHolder m_recordHolder;

        /// <summary>
        /// 移动回调
        /// </summary>
        public event BTrackerMoveHandler BTrackerMoveDelegete;


        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            // 
            Global.currentMainWindow = this;

            var host = System.Configuration.ConfigurationManager.AppSettings["rfid_server_host"];
            var port = System.Configuration.ConfigurationManager.AppSettings["rfid_server_port"];
            int iport;
            if (!int.TryParse(port, out iport))
            {
                iport = 7500;
            }

            m_rfidServer = new PFTSHwCtrl.PFTSRFIDServer(host, iport);
            m_rfidServer.Start();
            m_rfidServer.BTrackerMove += M_rfidServer_BTrackerMove;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(LoadThread));
            thread.Start();
        }

        private void LoadThread()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                m_videoRecorder = new PFTSHwCtrl.PFTSVideoRecordProxy();
                m_recordHolder = new VideoRecordHolder();
                m_recordHolder.Visibility = Visibility.Hidden;
                m_recordHolder.Show();
                m_videoRecorder.VideoRecordDelegate += M_videoRecorder_VideoRecordDelegate;
                m_videoRecorder.LoadRooms();
            });
        }

        public void StopRecord(PFTSHwCtrl.StopRecordCallback call)
        {
            m_videoRecorder.StopAll(call);
        }

        private void M_videoRecorder_VideoRecordDelegate(view_camera_info camera, PFTSHwCtrl.VideoRecordEvent e, PFTSHwCtrl.VideoRecordCallback callback)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                if (e == PFTSHwCtrl.VideoRecordEvent.StartRecord)
                {
                    m_recordHolder.StartRecord(camera);
                }
                else if (e == PFTSHwCtrl.VideoRecordEvent.ReRecord)
                {
                    var video = m_recordHolder.ReRecord(camera);
                    if (callback != null)
                    {
                        callback(video);
                    }
                }
                else {
                    var video = m_recordHolder.StopRecord(camera);
                    if (callback != null)
                    {
                        callback(video);
                    }
                }
            });
        }

        private void M_rfidServer_BTrackerMove(btracker btracker, view_rfid_info position)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                bool r = (new PFTSModel.Services.BTrackerService()).MoveTo(btracker.id, position);
                if (!r)
                {
                    MessageBox.Show(btracker.name + "移动至" + position.room_name + "失败");
                    PFTSTools.ConsoleManager.SetOut(btracker.name + "移动至->" + position.room_name + "失败");
                }
                else
                {
                    PFTSTools.ConsoleManager.SetOut(btracker.name + "移动至->" + position.room_name + "成功");
                    if (BTrackerMoveDelegete != null)
                        BTrackerMoveDelegete(btracker.id);
                    m_videoRecorder.BtrackerMoveTo(btracker.id, btracker.room_id, position.room_id.Value);
                }
            });
        }

        private void MainWindow_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            if (this.ResizeMode == ResizeMode.NoResize) return;

            //判断点击的控件不是Grid类型，就可以双击
            if (!(e.OriginalSource is System.Windows.Controls.Image))
            {
                return;
            }


            IInputElement feSource = e.MouseDevice.DirectlyOver;


            Point pt = e.MouseDevice.GetPosition(this);
            if (pt.Y < MoveHeight)
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
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSysMax_Click(object sender, RoutedEventArgs e)
        {
            return;

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
    }
}
