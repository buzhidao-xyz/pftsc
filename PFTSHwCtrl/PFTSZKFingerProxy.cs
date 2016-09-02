using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libzkfpcsharp;
using System.IO;
using System.Drawing;
using System.Threading;

namespace PFTSHwCtrl
{

    public delegate void FingerAcquireHander(Bitmap img,byte[] buffer);

    /// <summary>
    /// 只是识别器
    /// </summary>
    public class PFTSZKFingerProxy : IDisposable
    {
        private zkfp m_fpInstance;
        private bool m_bInit = false;
        private int m_devCount = 0;
        private bool m_bOpen = false;
        private bool m_bIsTimeToDie = false;
        /// <summary>
        /// 指纹图谱缓存
        /// </summary>
        private byte[] m_FPBuffer;
        private byte[] m_capTmp = new byte[2048];
        private int m_cbCmbTmp;


        public event FingerAcquireHander FingerAcquire;

        /// <summary>
        /// contructor
        /// </summary>
        public PFTSZKFingerProxy()
        {
            Init();
        }

        static PFTSZKFingerProxy m_instance;
        public static PFTSZKFingerProxy GetInstance()
        {
            if (m_instance == null)
            {
                m_instance = new PFTSZKFingerProxy();
                m_instance.Open();
                m_instance.StartAcquireThread();
            }
            return m_instance;
        }

        /// <summary>
        /// init
        /// </summary>
        private void Init()
        {
            m_fpInstance = new zkfp();
            int result = m_fpInstance.Initialize();
            if (result != 0)
            {
                throw new Exception("指纹采集器初始化失败 ErrorCode:"+result);
            }
            m_bInit = true;
            m_devCount = m_fpInstance.GetDeviceCount();
        }

        /// <summary>
        /// open device
        /// </summary>
        public void Open()
        {
            if (!m_bInit)
            {
                throw new Exception("在打开设备前需要初始化");
            }
            if (m_devCount <= 0)
            {
                throw new Exception("未找到指纹采集设备");
            }
            // open the first
            int result = m_fpInstance.OpenDevice(0);
            if (result != 0)
            {
                throw new Exception("打开指纹采集器设备失败 ErrorCode:" + result);
            }
            m_bOpen = true;
            m_FPBuffer = new byte[m_fpInstance.imageWidth * m_fpInstance.imageHeight];
        }

        /// <summary>
        /// close device
        /// </summary>
        public void Close()
        {
            if (m_bOpen)
            {
                m_fpInstance.CloseDevice();
            }
        }

        /// <summary>
        /// 采集指纹
        /// </summary>
        /// <returns></returns>
        public bool AcquireFingerprint()
        {
            m_cbCmbTmp = 2048;
            int ret = m_fpInstance.AcquireFingerprint(m_FPBuffer, m_capTmp, ref m_cbCmbTmp);
            return ret == 0;
        }

        /// <summary>
        /// 获取指纹数据
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public byte[] GetRaw(ref int size)
        {
            size = m_cbCmbTmp;
            byte[] nb = new byte[m_cbCmbTmp];
            try
            {
                Buffer.BlockCopy(m_capTmp, 0, nb,0, nb.Length);
                return nb;
            }
            catch
            {
                return m_capTmp;
            }
        }


        public void StartAcquireThread()
        {
            Thread captureThread = new Thread(new ThreadStart(DoCapture));
            captureThread.IsBackground = true;
            captureThread.Start();
            m_bIsTimeToDie = false;
        }

        private void DoCapture()
        {
            while (!m_bIsTimeToDie)
            {
                var b = AcquireFingerprint();
                if (b)
                {
                    var img = GetFingerImage();
                    int size = 2048;
                    var buffer = GetRaw(ref size);
                    if (FingerAcquire != null)
                    {
                        FingerAcquire(img, buffer);
                    }
                    //this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    //{
                    //    int size = 2048;
                    //    var img = m_fingerProxy.GetFingerImage();
                    //    var buffer = m_fingerProxy.GetRaw(ref size);
                    //    imgFinger.Source = PoliceNewPage.ChangeBitmapToImageSource(img);

                    //    var officer = m_model.GetPoliceInfo;
                    //    officer.fingerprint1 = new Binary(buffer);
                    //    m_model.GetPoliceInfo = officer;
                    //});
                }
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// 获取指纹图片
        /// </summary>
        /// <returns></returns>
        public Bitmap GetFingerImage()
        {
            MemoryStream ms = new MemoryStream();
            BitmapFormat.GetBitmap(m_FPBuffer, m_fpInstance.imageWidth, m_fpInstance.imageHeight, ref ms);
            Bitmap bmp = new Bitmap(ms);
            return bmp;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Close();
            if (m_bInit)
            {
                m_fpInstance.Finalize();
            }
        }
    }
}
