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

namespace PFTSDesktop.View.DeviceManager
{
    /// <summary>
    /// RFIDAddDlg.xaml 的交互逻辑
    /// </summary>
    public partial class RFIDAddDlg : WindowTemplet
    {
        public RFIDAddDlg()
        {
            InitializeComponent();
            this.DataContext = RFIDManagerViewModel.GetInstance();
        }
    }
}
