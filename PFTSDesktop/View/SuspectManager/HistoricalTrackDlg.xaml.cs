using PFTSDesktop.ViewModel;
using PFTSModel;
using PFTSUITemplate.Controls;
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

namespace PFTSDesktop.View.SuspectManager
{
    /// <summary>
    /// HistoricalTrackDlg.xaml 的交互逻辑
    /// </summary>
    public partial class HistoricalTrackDlg : WindowTemplet
    {
        public HistoricalTrackDlg()
        {
            InitializeComponent();

        }
        public HistoricalTrackDlg(btracker model)
        {
            InitializeComponent();
            this.DataContext = SuspectViewModel.GetInstance();

            // 监控模式
            controlScene.CameraMode = PFTSScene.CameraMode.Hidden;
            controlScene.RFIDMode = PFTSScene.RFIDMode.Hidden;

            controlScene.AddAPeople(model);
            controlScene.PathOut(model.id,false);
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
