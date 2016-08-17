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

namespace PFTSHwCtrl
{
    /// <summary>
    /// PFTSVideoControl.xaml 的交互逻辑
    /// </summary>
    public partial class PFTSVideoControl : UserControl
    {
        private bool m_bInitSDK = false;
        private string m_dvrIPAddress = "192.168.10.164";
        private Int16 m_dvrPortNumber = 8000;
        private string m_dvrUserName = "admin";
        private string m_dvrPassword = "Gt123456";
        // >0 登录
        private Int32 m_lUserID = -1;
        // >0 获取实时画面
        private Int32 m_lRealHandle = -1;

        private uint iLastErr = 0;
        private uint dwAChanTotalNum = 0;
        private uint dwDChanTotalNum = 0;
        public CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo;
        public PFTSVideoControl()
        {
            InitializeComponent();
            try
            {
                m_bInitSDK = CHCNetSDK.NET_DVR_Init();
                if (m_bInitSDK == false)
                {
                    MessageBox.Show("NET_DVR_Init error!");
                    return;
                }
                else
                {
                    //保存SDK日志 To save the SDK log
                    //CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
                    login();
                    if (m_lUserID >= 0)
                    {
                        preview();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void login()
        {
            //登录设备 Login the device
            m_lUserID = CHCNetSDK.NET_DVR_Login_V30(m_dvrIPAddress, m_dvrPortNumber, m_dvrUserName, m_dvrPassword, ref DeviceInfo);
            if (m_lUserID < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string str = "NET_DVR_Login_V30 failed, error code= " + iLastErr; //登录失败，输出错误号 Failed to login and output the error code
                MessageBox.Show(str);
                return;
            }
            else
            {
                //登录成功
                dwAChanTotalNum = (uint)DeviceInfo.byChanNum;
                dwDChanTotalNum = (uint)DeviceInfo.byIPChanNum + 256 * (uint)DeviceInfo.byHighDChanNum;
            }
            return;
        }

        private void preview() 
        {
            if (m_lRealHandle < 0)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = realPlayWnd.Handle; //    RealPlayWnd.Handle;//预览窗口 live view window
                lpPreviewInfo.lChannel = (int)DeviceInfo.byStartChan;//预览的设备通道 the device channel number
                lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 15; //播放库显示缓冲区最大帧数

                IntPtr pUser = IntPtr.Zero;//用户数据 user data 

                //if (comboBoxView.SelectedIndex == 0)
                //{
                    //打开预览 Start live view 
                    m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);
                //}
                //else
                //{
                //    lpPreviewInfo.hPlayWnd = IntPtr.Zero;//预览窗口 live view window
                //    m_ptrRealHandle = RealPlayWnd.Handle;
                //    RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数 real-time stream callback function 
                //    m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, RealData, pUser);
                //}

                if (m_lRealHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string str = "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr; //预览失败，输出错误号 failed to start live view, and output the error code.
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    //预览成功
                }
            }
        }
    }
}
