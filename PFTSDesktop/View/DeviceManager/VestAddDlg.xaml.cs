using PFTSDesktop.ViewModel;
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
using System.Windows.Threading;

namespace PFTSDesktop.View.DeviceManager
{
    /// <summary>
    /// VestAddDlg.xaml 的交互逻辑
    /// </summary>
    public partial class VestAddDlg : WindowTemplet
    {
        private bool m_bFocusNo1 = false;
        private bool m_bFocusNo2 = false;
        private PFTSHwCtrl.PFTSRFIDNoReaderProxy rfidReader;

        public VestAddDlg()
        {
            InitializeComponent();
            this.DataContext = VestManagerViewModel.GetInstance();
            var com = System.Configuration.ConfigurationManager.AppSettings["rfid_reader_com"];
            rfidReader = new PFTSHwCtrl.PFTSRFIDNoReaderProxy(com);
        }

        private void txtNo_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                rfidReader.Open();
                rfidReader.StartAcquireRFIDNo();
                rfidReader.RFIDNoReaderDelegate += Proxy_RFIDNoReaderDelegate;
                m_bFocusNo1 = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

        }

        private void Proxy_RFIDNoReaderDelegate(List<string> rfidNos)
        {
            //throw new NotImplementedException();
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate()
            {
                if (m_bFocusNo1)
                {
                    foreach (var l in rfidNos)
                    {
                        //MessageBox.Show(l);
                        txtNo.Text = l.Substring(0, 8);
                    }
                }
                else if (m_bFocusNo2)
                {
                    foreach (var l in rfidNos)
                    {
                        //MessageBox.Show(l);
                        txtNo2.Text = l.Substring(0, 8);
                    }
                }
            });
        }

        private void txtNo_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                rfidReader.RFIDNoReaderDelegate -= Proxy_RFIDNoReaderDelegate;
                rfidReader.EndAcquireRFIDNo();
                rfidReader.Close();
                m_bFocusNo1 = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private void txtNo2_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                rfidReader.Open();
                rfidReader.StartAcquireRFIDNo();
                rfidReader.RFIDNoReaderDelegate += Proxy_RFIDNoReaderDelegate;
                m_bFocusNo2 = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private void txtNo2_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                rfidReader.RFIDNoReaderDelegate -= Proxy_RFIDNoReaderDelegate;
                rfidReader.EndAcquireRFIDNo();
                rfidReader.Close();
                m_bFocusNo2 = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }
}
