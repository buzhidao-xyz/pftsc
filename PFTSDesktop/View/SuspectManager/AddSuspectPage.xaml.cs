using PFTSDesktop.ViewModel;
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

namespace PFTSDesktop.View.SuspectManager
{
    /// <summary>
    /// AddSuspectPage.xaml 的交互逻辑
    /// </summary>
    public partial class AddSuspectPage : Page
    {
        private PFTSHwCtrl.PFTSRFIDNoReaderProxy rfidReader;
        private string m_lastVestNo;
        private SuspectViewModel m_model;

        public AddSuspectPage()
        {
            InitializeComponent();
            m_model = new SuspectViewModel();
            this.DataContext = m_model;
            var com = System.Configuration.ConfigurationManager.AppSettings["rfid_reader_com"];
            rfidReader = new PFTSHwCtrl.PFTSRFIDNoReaderProxy(com);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                rfidReader.Open();
                rfidReader.StartAcquireRFIDNo();
                rfidReader.RFIDNoReaderDelegate += RfidReader_RFIDNoReaderDelegate;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private void RfidReader_RFIDNoReaderDelegate(List<string> rfidNos)
        {
            foreach (var l in rfidNos)
            {
                if (m_lastVestNo == l) { continue; }
                //TODO: for test
                var no = l.Substring(0, 8);
                var vest = (new PFTSModel.DevVestService()).GetByNo(no, null, null);
                if (vest != null)
                {
                    foreach (var v in m_model.DevVests)
                    {
                        if (v.id == vest.id)
                        {
                            m_model.DevVest = v;
                            break;
                        }
                    }
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                rfidReader.RFIDNoReaderDelegate -= RfidReader_RFIDNoReaderDelegate;
                rfidReader.EndAcquireRFIDNo();
                rfidReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }
}
