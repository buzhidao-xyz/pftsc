using PFTSDesktop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PFTSDesktop
{
    public class Global
    {
        /// <summary>
        /// 页面显示容器
        /// </summary>
        public static Frame currentFrame { get; set; }

        /// <summary>
        /// 程序主窗体
        /// </summary>
        public static MainWindow currentMainWindow { get; set; }

    }
}
