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

namespace PFTSDesktop
{
    /// <summary>
    /// LoginDlg.xaml 的交互逻辑
    /// </summary>
    public partial class LoginDlg : WindowTemplet
    {
        public LoginDlg()
        {
            InitializeComponent();
        }

        private void main_Loaded(object sender, RoutedEventArgs e)
        {
            //PFTSHwCtrl.PFTSRFIDNoReaderProxy proxy = new PFTSHwCtrl.PFTSRFIDNoReaderProxy("COM3");
            //proxy.Open();
            //proxy.StartAcquireRFIDNo();
            //proxy.RFIDNoReaderDelegate += Proxy_RFIDNoReaderDelegate;
        }

        //private void Proxy_RFIDNoReaderDelegate(List<string> rfidNos)
        //{
        //    //throw new NotImplementedException();
        //    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (System.Threading.ThreadStart)delegate ()
        //    {
        //        foreach (var l in rfidNos)
        //        {
        //            MessageBox.Show(l);
        //        }
        //    });
        //}
    }
}
