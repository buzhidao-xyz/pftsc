using PFTSDesktop.ViewModel;
using PFTSHwCtrl;
using PFTSModel;
using PFTSUITemplate.Controls;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace PFTSDesktop.View.PoliceManager
{
    /// <summary>
    /// PoliceUpDlg.xaml 的交互逻辑
    /// </summary>
    public partial class PoliceUpDlg : WindowTemplet
    {
        PFTSHwCtrl.PFTSZKFingerProxy m_fingerProxy;
        private bool m_bIsTimeToDie = false;
        private PoliceManagerViewModel m_model;
        private int m_idx = -1;

        public PoliceUpDlg()
        {
            InitializeComponent();
            m_model = PoliceManagerViewModel.GetInstance();

            this.DataContext = m_model;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                List<officer_fingerprint> fingerList = m_model.OfficerFingerList;
                if (fingerList != null && fingerList.Count > 0)
                {
                    foreach (officer_fingerprint model in fingerList)
                    {
                        if (model.img_height != null && model.img_width != null)
                        {
                            if (model.finger_id == 1)
                            {
                                Bitmap img = GetFingerImageByData(model.img.ToArray(), model.img_width.Value, model.img_height.Value);
                                imgFinger1.Source = ChangeBitmapToImageSource(img);
                            }
                            else if (model.finger_id == 2)
                            {
                                Bitmap img = GetFingerImageByData(model.img.ToArray(), model.img_width.Value, model.img_height.Value);
                                imgFinger2.Source = ChangeBitmapToImageSource(img);
                            }
                        }
                    }
                }
                m_fingerProxy = PFTSHwCtrl.PFTSZKFingerProxy.GetInstance();
                m_fingerProxy.FingerAcquire += M_fingerProxy_FingerAcquire;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 获取指纹图片
        /// </summary>
        /// <returns></returns>
        public Bitmap GetFingerImageByData(byte[] data, int imageWidth, int imageHeight)
        {
            MemoryStream ms = new MemoryStream();
            BitmapFormat.GetBitmap(data, imageWidth, imageHeight, ref ms);
            Bitmap bmp = new Bitmap(ms);
            return bmp;
        }

        private void M_fingerProxy_FingerAcquire(Bitmap img, byte[] buffer, byte[] imgBuffer, int imageHeight, int imageWidth)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
            {
                if (m_idx == 1)
                {
                    imgFinger1.Source = ChangeBitmapToImageSource(img);
                    var officer = m_model.GetPoliceInfo;
                    officer.fingerprint1 = new Binary(buffer);
                    m_model.GetPoliceInfo = officer;
                    bool bExist = false;
                    for (int i = 0; i < m_model.OfficerFingerList.Count; i++)
                    {
                        if (m_model.OfficerFingerList[i].finger_id == 1)
                        {
                            m_model.OfficerFingerList[i].img = new System.Data.Linq.Binary(imgBuffer);
                            bExist = true;
                            break;
                        }
                    }
                    if (!bExist)
                    {
                        PFTSModel.officer_fingerprint fp = new PFTSModel.officer_fingerprint();
                        fp.finger_id = 1;
                        fp.officer_id = officer.id;
                        fp.img_height = imageHeight;
                        fp.img_width = imageWidth;
                        fp.img = new System.Data.Linq.Binary(imgBuffer);
                        m_model.OfficerFingerList.Add(fp);
                    }
                }
                else if (m_idx == 2)
                {
                    imgFinger2.Source = ChangeBitmapToImageSource(img);
                    var officer = m_model.GetPoliceInfo;
                    officer.fingerprint2 = new Binary(buffer);
                    m_model.GetPoliceInfo = officer;
                    bool bExist = false;
                   for (int i = 0; i < m_model.OfficerFingerList.Count; i++)
                    {
                        if (m_model.OfficerFingerList[i].finger_id == 2)
                        {
                            m_model.OfficerFingerList[i].img = new System.Data.Linq.Binary(imgBuffer);
                            bExist = true;
                            break;
                        }
                    }
                    if (!bExist)
                    {
                        PFTSModel.officer_fingerprint fp = new PFTSModel.officer_fingerprint();
                        fp.finger_id = 2;
                        fp.officer_id = officer.id;
                        fp.img_height = imageHeight;
                        fp.img_width = imageWidth;
                        fp.img = new System.Data.Linq.Binary(imgBuffer);
                        m_model.OfficerFingerList.Add(fp);
                    }
                }
            });
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>  
        /// 从bitmap转换成ImageSource  
        /// </summary>  
        /// <param name="icon"></param>  
        /// <returns></returns>  
        public static ImageSource ChangeBitmapToImageSource(Bitmap bitmap)
        {
            //Bitmap bitmap = icon.ToBitmap();  
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            if (!DeleteObject(hBitmap))
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return wpfBitmap;
        }

        /// <summary>
        /// 鼠标点击事件 - 左键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgFinger1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imgFinger1_Border.BorderBrush = System.Windows.Media.Brushes.Blue;
            imgFinger2_Border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, 0xab, 0xad, 0xb3));
            m_idx = 1;
        }

        private void imgFinger2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imgFinger1_Border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, 0xab, 0xad, 0xb3));
            imgFinger2_Border.BorderBrush = System.Windows.Media.Brushes.Blue;
            m_idx = 2;
        }
    }
}
