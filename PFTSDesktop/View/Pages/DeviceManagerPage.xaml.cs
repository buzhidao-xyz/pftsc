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

namespace PFTSDesktop.View.Pages
{
    /// <summary>
    /// DeviceManagerPage.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceManagerPage : Page
    {
        public DeviceManagerPage()
        {
            InitializeComponent();
            this.DataContext = new DeviceManagerViewModel();
        }
    }
}
