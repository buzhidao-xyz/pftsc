using GFramework.BlankWindow;
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

namespace PFTSDesktop.View.Monitoring
{
    /// <summary>
    /// VideoListWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VideoListWindow : BlankWindow
    {
        private PFTSHwCtrl.PFTSVideoProxy m_videoProxy;
        private PFTSModel.dev_camera m_camera;

        public VideoListWindow(PFTSModel.dev_camera camera = null)
        {
            InitializeComponent();
            m_camera = camera;
            //            m_videoProxy = new PFTSHwCtrl.PFTSVideoProxy("192.168.10.164", 8000, "admin", "Gt123456");

            timer.Tick += new EventHandler(timer_Tick);


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 实时画面
            if (m_camera != null)
            {
                m_videoProxy = new PFTSHwCtrl.PFTSVideoProxy(m_camera.ip, m_camera.port == null ? 8000 : m_camera.port.Value, m_camera.admin, m_camera.password);
                m_videoProxy.Login();
                m_videoProxy.SetWindow(videoControl);
                if (!m_videoProxy.IsRealView)
                {
                    if (!m_videoProxy.StartRealView())
                    {
                        MessageBox.Show(m_videoProxy.GetLastError());
                    }
                }
                //volumeSlider.Value = 0.5;
            }
        }

        //
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            videoControl.Visibility = Visibility.Hidden;
            moviePlayerGrid.Visibility = Visibility.Visible;
            PlayMovie(new Uri("D:\\fff111.mp4", UriKind.Absolute));
        }

        //real
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            videoControl.Visibility = Visibility.Visible;
            moviePlayerGrid.Visibility = Visibility.Hidden;
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
            this.Close();
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

        #region 打开视频文件

        // Specifies whether the movie is playing.
        private bool playing;

        // Used to update the position slider's current value.
        private System.Windows.Threading.DispatcherTimer timer =
            new System.Windows.Threading.DispatcherTimer();

        //播放视频
        public void PlayMovie(Uri movie)
        {
            mediaElement.Source = movie;
            PlayMovie();
        }

        //关闭视频
        public void CloseMedia()
        {
            StopMovie();
            mediaElement.Close();
        }



        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moviePlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            //mediaElement.Volume = (double)volumeSlider.Value;
            positionSlider.Maximum =
              mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            string totaltime = mediaElement.NaturalDuration.ToString().Substring(0, 8);
            showTime.Text = "00:00:00" + "/" + totaltime;
        }

        /// <summary>
        /// 停止播放视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moviePlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            StopMovie();
        }

        /// <summary>
        /// 视屏进度条变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void positionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Create a TimeSpan with milliseconds equal to the slider value.
            TimeSpan ts = new TimeSpan(
              0, 0, 0, 0, (int)positionSlider.Value);
            string currentTime = ts.ToString().Substring(0, 8);
            string totaltime = mediaElement.NaturalDuration.ToString().Substring(0, 8);
            showTime.Text = currentTime + "/" + totaltime;
            mediaElement.Position = ts;
        }
        /// <summary>
        /// 后退
        /// </summary>
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            // Jump back 5 seconds:
            mediaElement.Position =
              mediaElement.Position.Subtract(new TimeSpan(0, 0, 0, 0, 5000));

            positionSlider.Value =
                mediaElement.Position.TotalMilliseconds;
        }

        /// <summary>
        /// 播放、暂停视频
        /// </summary>
        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            PlayMovie();
        }

        /// <summary>
        /// 停止视频播放
        /// </summary>
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            StopMovie();
        }

        /// <summary>
        /// 前进
        /// </summary>
        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            // Jump ahead 5 seconds:
            mediaElement.Position =
              mediaElement.Position.Add(new TimeSpan(0, 0, 0, 0, 5000));

            positionSlider.Value =
              mediaElement.Position.TotalMilliseconds;
        }

        /// <summary>
        /// 声音大小进度条变化事件
        /// </summary>
        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //mediaElement.Volume = (double)volumeSlider.Value;
        }

        #region 公共方法

        void timer_Tick(object sender, EventArgs e)
        {
            positionSlider.Value =
              mediaElement.Position.TotalMilliseconds;
        }

        private void PlayMovie()
        {
            ControlTemplate tmpl;
            if (!playing)
            {
                mediaElement.Play();
                tmpl = (ControlTemplate)this.FindResource("PauseButton");
                playButton.Template = tmpl;
                playButton.ToolTip = "暂停";
                playing = true;
            }
            else
            {
                mediaElement.Pause();
                tmpl = (ControlTemplate)this.FindResource("PlayButton");
                playButton.Template = tmpl;
                playButton.ToolTip = "播放";
                playing = false;
            }
        }

        private void StopMovie()
        {
            mediaElement.Stop();
            mediaElement.Position = TimeSpan.Zero;
            positionSlider.Value =
              mediaElement.Position.TotalMilliseconds;
            ControlTemplate tmpl = (ControlTemplate)this.FindResource("PlayButton");
            playButton.Template = tmpl;
            playButton.ToolTip = "播放";
            playing = false;
        }

        #endregion
        #endregion
    }
}
