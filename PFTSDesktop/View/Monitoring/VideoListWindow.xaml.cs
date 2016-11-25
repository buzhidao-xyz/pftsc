using GFramework.BlankWindow;
using Microsoft.Win32;
using PFTSDesktop.common;
using PFTSDesktop.Model;
using PFTSModel;
using PFTSModel.Services;
using PFTSTools;
using PFTSUITemplate.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PFTSDesktop.View.Monitoring
{
    /// <summary>
    /// VideoListWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VideoListWindow : BlankWindow
    {
        private PFTSHwCtrl.PFTSVideoProxy m_videoProxy;
        private PFTSModel.dev_camera m_camera;
        private BTrackerService service;
        private int? btracker_id;
        private List<VideoModel> videoList;
        private int camera_id = 0;
        private string totalTime;
        private double maxTime;

        public VideoListWindow(PFTSModel.dev_camera camera = null, int? btracker_id = null)
        {
            InitializeComponent();
            m_camera = camera;
            this.btracker_id = btracker_id;
            videoList = new List<VideoModel>();
            VideoModel videoModel = new VideoModel();
            videoModel.id = 0;
            videoModel.video_name = "实时画面";
            videoList.Add(videoModel);
            timer.Tick += new EventHandler(timer_Tick);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            service = new BTrackerService();
            if (btracker_id != null)
            {
                List<view_btracker_video> videos = service.GetVideoByBtracker(btracker_id.Value);
                if (videos != null && videos.Count > 0)
                {
                    foreach (view_btracker_video entity in videos)
                    {
                        long timeRange = (entity.end_time.Ticks - entity.start_time.Ticks) / 10000 / 1000;
                        if (Convert.ToInt32(timeRange) > 20)
                        {
                            if (entity.camera_id == camera_id)
                            {
                                VideoModel videoModel = videoList.Find(c => c.id == camera_id);
                                videoModel.video_time += (entity.end_time.Ticks - entity.start_time.Ticks) / 10000;
                                videoModel.videoTimes.Add(new TimeSpan(0, 0, Convert.ToInt32(timeRange)));
                                videoModel.videos.Add(entity.filename);
                            }
                            else
                            {
                                VideoModel videoModel = new VideoModel();
                                videoModel.id = entity.camera_id;
                                videoModel.video_name = entity.video_name;
                                videoModel.video_time = (entity.end_time.Ticks - entity.start_time.Ticks) / 10000;
                                videoModel.videos = new List<string>();
                                videoModel.videos.Add(entity.filename);
                                videoModel.videoTimes = new List<TimeSpan>();

                                videoModel.videoTimes.Add(new TimeSpan(0, 0, Convert.ToInt32(timeRange)));
                                videoList.Add(videoModel);
                            }
                            camera_id = entity.camera_id;
                        }
                    }
                }

            }
            listBox.ItemsSource = videoList;
            listBox.SelectedIndex = 0;
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
                        MessageBox.Show("暂时无法连接到摄像头，请检查摄像头状态！"); // + m_videoProxy.GetLastError()
                    }
                }
                //volumeSlider.Value = 0.5;
            }
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
            StopMovie();
            mediaElement.Close();
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


        private string preTotalTime;
        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moviePlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            //mediaElement.Volume = (double)volumeSlider.Value;
            positionSlider.Maximum = maxTime;
            //mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            //string totaltime = mediaElement.NaturalDuration.ToString().Substring(0, 8);
            preTotalTime = mediaElement.NaturalDuration.ToString().Substring(0, 8);
            if (i == 0)
            {
                showTime.Text = "00:00:00" + "/" + totalTime;
            }
            else
            {
                showTime.Text = preTotalTime + "/" + totalTime;
            }
        }

        /// <summary>
        /// 停止播放视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moviePlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            i++;
            if (i >= videoModel.videos.Count)
            {
                i = 0;
                StopMovie();
            }
            else
            {
                //mediaElement.Stop();
                playing = false;
                PlayMovie(new Uri(videoModel.videos[i], UriKind.Absolute));
            }
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
            //string totaltime = mediaElement.NaturalDuration.ToString().Substring(0, 8);
            showTime.Text = currentTime + "/" + totalTime;
            if (i == 0)
            {
                mediaElement.Position = ts;
            }
            else
            {
                TimeSpan temp = new TimeSpan();
                int index = -1;
                for (int m = 0; m < videoModel.videoTimes.Count(); m++)
                {
                    temp += videoModel.videoTimes[m];
                    if (ts < temp)
                    {
                        index = m;
                        break;
                    }
                }

                if (index != i && index != -1)
                {
                    i = index;
                    playing = false;
                    PlayMovie(new Uri(videoModel.videos[i], UriKind.Absolute));
                }
                TimeSpan length = new TimeSpan();
                for (int m = 0; m < i; m++)
                {
                    length += videoModel.videoTimes[m];
                }
                mediaElement.Position = ts - length;
            }
        }
        /// <summary>
        /// 后退
        /// </summary>
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan temp = mediaElement.Position.Subtract(new TimeSpan(0, 0, 0, 0, 5000));
            if (i == 0)
            {
                // Jump back 5 seconds:
                mediaElement.Position =
                  mediaElement.Position.Subtract(new TimeSpan(0, 0, 0, 0, 5000));
                positionSlider.Value =
               mediaElement.Position.TotalMilliseconds;
            }
            else
            {
                if (temp.TotalSeconds < 0)
                {
                    i--;
                    playing = false;
                    PlayMovie(new Uri(videoModel.videos[i], UriKind.Absolute));
                    mediaElement.Position = videoModel.videoTimes[i] + temp;
                }
                else
                {
                    mediaElement.Position = videoModel.videoTimes[i] + temp;
                }
                positionSlider.Value -= 5000;
            }

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
            TimeSpan temp = mediaElement.Position.Add(new TimeSpan(0, 0, 0, 0, 5000));
            if (i == videoModel.videoTimes.Count - 1)
            {
                // Jump ahead 5 seconds:
                mediaElement.Position =
                  mediaElement.Position.Add(new TimeSpan(0, 0, 0, 0, 5000));

                positionSlider.Value =
                  mediaElement.Position.TotalMilliseconds;
            }
            else
            {
                if (temp.TotalSeconds > mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds)
                {
                    i++;
                    playing = false;
                    PlayMovie(new Uri(videoModel.videos[i], UriKind.Absolute));
                    mediaElement.Position = videoModel.videoTimes[i] + temp - mediaElement.NaturalDuration.TimeSpan;

                }
                else
                {
                    mediaElement.Position = videoModel.videoTimes[i] + temp;
                }
                positionSlider.Value += 5000;
            }


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
            if (i == 0)
            {
                positionSlider.Value =
                  mediaElement.Position.TotalMilliseconds;
            }
            else
            {
                if (videoModel.id != 0)
                {
                    double temp = 0;
                    for (int m = 0; m < i; m++)
                    {
                        temp += videoModel.videoTimes[m].TotalMilliseconds;
                    }
                    positionSlider.Value =
                     temp + mediaElement.Position.TotalMilliseconds;
                }
            }
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
        VideoModel videoModel;
        int i = 0;
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StopMovie();
            videoModel = listBox.SelectedItem as VideoModel;
            if (videoModel.id == 0)
            {
                videoControl.Visibility = Visibility.Visible;
                moviePlayerGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                i = 0;
                videoControl.Visibility = Visibility.Hidden;
                moviePlayerGrid.Visibility = Visibility.Visible;
                totalTime = videoModel.video_Date;
                maxTime = videoModel.video_time;
                PlayMovie(new Uri(videoModel.videos[i], UriKind.Absolute));
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            videoModel = listBox.SelectedItem as VideoModel;
            if (videoModel.id != 0)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".mp4"; // Default file extension
                dlg.Filter = "mp4文件(.mp4)|*.mp4"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();
                string filename = string.Empty;

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    filename = dlg.FileName;

                    // Result could be true, false, or null

                    VideoConcatTool tool = new VideoConcatTool();

                    //        tool.ReportProgress += (idx, total) =>
                    //        {
                    //            pro.Dispatcher.Invoke(new Action<System.Windows.DependencyProperty, object>(pro.SetValue),
                    //System.Windows.Threading.DispatcherPriority.Background, ProgressBar.ValueProperty, Math.Round(100 * (double)idx / (double)total, 0));
                    //            //pro.Value = 100 * (float)idx / (float)total;
                    //        };

                    tool.ConcatVideo(videoModel.videos, filename);

                }
                else
                {
                    return;
                }
            }

        }

        private void listBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            object item = GetElementFromPoint((ItemsControl)sender, e.GetPosition((ItemsControl)sender));
            if (item != null)
            {
                listBox.ContextMenu.Opacity = 1;
            }
            else
            {
                listBox.ContextMenu.Opacity = 0;
            }
        }
        private object GetElementFromPoint(ItemsControl itemsControl, Point point)
        {
            UIElement element = itemsControl.InputHitTest(point) as UIElement;
            while (element != null)
            {
                if (element == itemsControl)
                    return null;
                object item = itemsControl.ItemContainerGenerator.ItemFromContainer(element);
                if (!item.Equals(DependencyProperty.UnsetValue))
                    return item;
                element = (UIElement)VisualTreeHelper.GetParent(element);
            }
            return null;
        }
    }
}
