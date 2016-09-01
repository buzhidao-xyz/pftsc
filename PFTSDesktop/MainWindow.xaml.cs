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

        /// <summary>
        /// 移动回调
        /// </summary>
        public event BTrackerMoveHandler BTrackerMoveDelegete;


        public MainWindow()
        {
            InitializeComponent();
            // 
            Global.currentMainWindow = this;

            m_rfidServer = new PFTSHwCtrl.PFTSRFIDServer("0.0.0.0",7500);
            m_rfidServer.Start();
            m_rfidServer.BTrackerMove += M_rfidServer_BTrackerMove;
        }

        private void M_rfidServer_BTrackerMove(btracker btracker, view_rfid_info position)
        {
            bool r = (new PFTSModel.Services.BTrackerService()).MoveTo(btracker.id, position);
            if (!r)
            {
                MessageBox.Show(btracker.name + "移动至" + position.room_name + "失败");
            }else
            {
//                MessageBox.Show(btracker.name + "移动到了" + position.room_name);
                if (BTrackerMoveDelegete != null)
                    BTrackerMoveDelegete(btracker.id);
            }
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
