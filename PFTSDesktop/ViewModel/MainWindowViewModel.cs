using PFTSDesktop.Command;
using PFTSUITemplate.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PFTSDesktop.ViewModel
{
    public class MainWindowViewModel : WorkspaceViewModel
    {
        private FButton preBtn;
        RelayCommand _openCommand;

        #region Commands
        public ICommand OpenCommand
        {
            get
            {
                if (_openCommand == null)
                {
                    _openCommand = new RelayCommand(
                        new Action<Object>(this.OpenPage));
                }
                return _openCommand;
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 加载工作区域
        /// </summary>
        /// <param name="obj">工作区路径</param>
        public void OpenPage(Object obj)
        {
            FButton btn = (FButton)obj;
            if (preBtn != null)
            {
                if (btn == preBtn)
                    return;
                preBtn.SelectEd = false;
            }
            preBtn = btn;
            btn.SelectEd = true;
            FrameSource = new Uri(btn.Tag.ToString(), UriKind.Relative);
        }
        #endregion
       
    }
}
