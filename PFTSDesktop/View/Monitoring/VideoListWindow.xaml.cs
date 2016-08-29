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
    public partial class VideoListWindow : Window
    {
        private PFTSHwCtrl.PFTSVideoProxy m_videoProxy;

        public VideoListWindow()
        {
            InitializeComponent();
            m_videoProxy = new PFTSHwCtrl.PFTSVideoProxy("192.168.10.164", 8000, "admin", "Gt123456");
            m_videoProxy.Login();
            m_videoProxy.SetWindow(videoControl);
            mediaElement.Source = new Uri("D:\\fff111.mp4", UriKind.Absolute);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!m_videoProxy.IsRealView)
            {
                if (!m_videoProxy.StartRealView())
                {
                    MessageBox.Show(m_videoProxy.GetLastError());
                }
            }
        }

        //
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            videoControl.Visibility = Visibility.Hidden;
            mediaElement.Visibility = Visibility.Visible;
            mediaElement.LoadedBehavior = MediaState.Manual;
            mediaElement.Play();
        }

        //real
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            videoControl.Visibility = Visibility.Visible;
            mediaElement.Visibility = Visibility.Hidden;
        }
    }
}
