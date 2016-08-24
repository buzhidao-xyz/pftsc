using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PFTSDesktop
{
	/// <summary>
	/// Video.xaml 的交互逻辑
	/// </summary>
	public partial class Video : Window
	{
        private PFTSHwCtrl.PFTSVideoProxy m_videoProxy;

        public Video()
		{
			this.InitializeComponent();
            m_videoProxy = new PFTSHwCtrl.PFTSVideoProxy("192.168.10.164", 8000, "admin", "Gt123456");
            m_videoProxy.Login();
            m_videoProxy.SetWindow(videoControl);

            // 在此点之下插入创建对象所需的代码。
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_videoProxy.IsRecording)
            {
                if (!m_videoProxy.StopRecord())
                {
                    MessageBox.Show(m_videoProxy.GetLastError());
                }
            }
            else
            {
                if (!m_videoProxy.StartRecord("d:\\fff111.mp4"))
                {
                    MessageBox.Show(m_videoProxy.GetLastError());
                }
            }

            if (m_videoProxy.IsRecording)
            {
                btn_record.Content = "停止录制";
            }
            else
            {
                btn_record.Content = "开始录制";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (m_videoProxy.IsRealView)
            {
                if (!m_videoProxy.StopRealView())
                {
                    MessageBox.Show(m_videoProxy.GetLastError());
                }
            }else
            {
                if (!m_videoProxy.StartRealView())
                {
                    MessageBox.Show(m_videoProxy.GetLastError());
                }
            }

            if (m_videoProxy.IsRealView)
            {
                btn_real.Content = "停止实时";
            }else
            {
                btn_real.Content = "开始实时";
            }
        }
    }
}