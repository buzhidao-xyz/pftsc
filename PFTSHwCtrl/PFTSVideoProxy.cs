using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSHwCtrl
{
    /// <summary>
    /// 海康威视视频控制接口
    /// </summary>
    public class PFTSVideoProxy
    {
        // param
        private string m_ipAddress;
        private Int16 m_port;
        private string m_userName;
        private string m_password;

        private bool m_bInitSDK = false;
        // 视频录制
        private bool m_bRecord = false;
        // >0 登录
        private Int32 m_lUserID = -1;
        private IntPtr m_hPlayWnd;
        // >0 获取实时画面
        private Int32 m_lRealHandle = -1;
        public CHCNetSDK.NET_DVR_DEVICEINFO_V30 m_deviceInfo;

        private uint m_iLastErr = 0;
        private uint m_dwAChanTotalNum = 0;
        private uint m_dwDChanTotalNum = 0;
        private string m_lastErr;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">摄像头ip</param>
        /// <param name="port">摄像头端口</param>
        /// <param name="username">摄像头用户名</param>
        /// <param name="password">摄像头密码</param>
        public PFTSVideoProxy(string ip, int port,string username,string password)
        {
            this.m_ipAddress =ip;
            this.m_port = (Int16)port;
            this.m_userName = username;
            this.m_password = password;
        }

        /// <summary>
        /// 获取上一次错误信息返回
        /// </summary>
        /// <returns></returns>
        public string GetLastError()
        {
            return m_lastErr;
        }

        /// <summary>
        /// 初始化摄像头连接
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            if (m_bInitSDK) return true;
            try
            {
                m_bInitSDK = CHCNetSDK.NET_DVR_Init();
                if (m_bInitSDK == false)
                {
                    m_lastErr = "NET_DVR_Init error!";
                }
            }
            catch (Exception e)
            {
                m_lastErr = e.Message;
                m_bInitSDK = false;
            }
            return m_bInitSDK;
        }

        /// <summary>
        /// 摄像头连接
        /// </summary>
        /// <returns></returns>
        public bool Login()
        {
            Init();
            //登录设备 Login the device
            m_lUserID = CHCNetSDK.NET_DVR_Login_V30(m_ipAddress, m_port, m_userName, m_password, ref m_deviceInfo);
            if (m_lUserID < 0)
            {
                m_iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                m_lastErr = "NET_DVR_Login_V30 failed, error code= " + m_iLastErr; //登录失败，输出错误号 Failed to login and output the error code
            }
            else
            {
                //登录成功
                m_dwAChanTotalNum = (uint)m_deviceInfo.byChanNum;
                m_dwDChanTotalNum = (uint)m_deviceInfo.byIPChanNum + 256 * (uint)m_deviceInfo.byHighDChanNum;
            }
            return m_lUserID >= 0;
        }

        /// <summary>
        /// 设置承载窗体控件
        /// </summary>
        /// <param name="control"></param>
        public void SetWindow(PFTSVideoControl control)
        {
            m_hPlayWnd = control.realPlayWnd.Handle;
        }

        /// <summary>
        /// 设置视频句柄窗口
        /// </summary>
        /// <param name="control"></param>
        public void SetVideoHandle(IntPtr handle)
        {
            m_hPlayWnd = handle;
        }

        /// <summary>
        /// 开始实时画面
        /// </summary>
        public bool StartRealView()
        {
            if (m_lRealHandle < 0)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = m_hPlayWnd; //    RealPlayWnd.Handle;//预览窗口 live view window
                lpPreviewInfo.lChannel = (int)m_deviceInfo.byStartChan;//预览的设备通道 the device channel number
                lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 15; //播放库显示缓冲区最大帧数

                IntPtr pUser = IntPtr.Zero;//用户数据 user data 

                m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);

                if (m_lRealHandle < 0)
                {
                    m_iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    m_lastErr = "NET_DVR_RealPlay_V40 failed, error code= " + m_iLastErr; //预览失败，输出错误号 failed to start live view, and output the error code.
                    return false;
                }
                else
                {
                    //预览成功
                    return true;
                }
            }
            return true;
        }

        /// <summary>
        /// 停止实时画面
        /// </summary>
        /// <returns></returns>
        public bool StopRealView()
        {
            if (m_lRealHandle < 0) return true;
            if (!CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle))
            {
                m_iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                m_lastErr = "NET_DVR_StopRealPlay failed, error code= " + m_iLastErr;
                return false;
            }
            m_lRealHandle = -1;
            return true;
        }

        /// <summary>
        /// 当前是否处于实时录制状态
        /// </summary>
        public bool IsRealView
        {
            get
            {
                return m_lRealHandle >= 0;
            }
        }

        /// <summary>
        /// 开始录制
        /// </summary>
        /// <param name="filename">录制保存的视频文件名</param>
        /// <returns></returns>
        public bool StartRecord(string filename)
        {
            if (m_lUserID < 0)
            {
                m_lastErr = "had not login";
                return false;
            }

            if (!m_bRecord)
            {
                //强制I帧 Make a I frame
                int lChannel = (int)m_deviceInfo.byStartChan; //通道号 Channel number
                CHCNetSDK.NET_DVR_MakeKeyFrame(m_lUserID, lChannel);

                //开始录像 Start recording
                //if (!CHCNetSDK.NET_DVR_SaveRealData(m_lRealHandle, sVideoFileName))
                if (!CHCNetSDK.NET_DVR_SaveRealData_V30(m_lRealHandle, 2, filename))
                {
                    m_iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    m_lastErr = "NET_DVR_SaveRealData failed, error code= " + m_iLastErr;
                    m_bRecord = false;
                }
                else
                {
                    m_bRecord = true;
                }
            }
            return m_bRecord;
        }

        /// <summary>
        /// 停止视频录制
        /// </summary>
        /// <returns></returns>
        public bool StopRecord()
        {
            if (!m_bRecord) return true;
            if (!CHCNetSDK.NET_DVR_StopSaveRealData(m_lRealHandle))
            {
                m_iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                m_lastErr = "NET_DVR_StopSaveRealData failed, error code= " + m_iLastErr;
                return false;
            }
            else
            {
                //m_lastErr = "Successful to stop recording and the saved file is " + sVideoFileName;
                m_bRecord = false;
                return true;
            }
        }

        /// <summary>
        /// 保存当前录制的视频
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool SaveRecord(string filename)
        {
            if (!m_bRecord) return true;
            if (!CHCNetSDK.NET_DVR_SaveRealData(m_lRealHandle,filename))
            {
                m_iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                m_lastErr = "NET_DVR_StopSaveRealData failed, error code= " + m_iLastErr;
                return false;
            }
            else
            {
                //m_lastErr = "Successful to stop recording and the saved file is " + sVideoFileName;
                m_bRecord = false;
                return true;
            }
        }

        /// <summary>
        /// 当前是否处于视频录制状态
        /// </summary>
        public bool IsRecording
        {
            get
            {
                return m_bRecord;
            }
        }
    }
}
