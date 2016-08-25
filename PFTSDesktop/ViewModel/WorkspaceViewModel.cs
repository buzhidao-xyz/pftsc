using PFTSDesktop.Command;
using PFTSUITemplate.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PFTSDesktop.ViewModel
{
    /// <summary>
    /// This ViewModelBase subclass requests to be removed 
    /// from the UI when its CloseCommand executes.
    /// This class is abstract.
    /// </summary>
    public abstract class WorkspaceViewModel : ViewModelBase
    {
        #region Fields

        RelayCommand _closeCommand;



        #endregion // Fields

        #region Constructor

        protected WorkspaceViewModel()
        {
        }

        #endregion // Constructor

        #region Commands

        /// <summary>
        /// Returns the command that, when invoked, attempts
        /// to remove this workspace from the user interface.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand(new Action<Object>(this.windowClose));
            }
        }

        public ICommand WindowMinCommand
        {
            get
            {
                return new RelayCommand(new Action<Object>(this.WindowMin));
            }
        }

        public ICommand PageCloseCommand
        {
            get
            {
                return new RelayCommand(param => this.PageClose());
            }
        }
        #endregion // CloseCommand

        #region RequestClose [event]

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        public void windowClose(Object obj)
        {
            WindowTemplet win = (WindowTemplet)obj;
            win.Close();
        }

        public void PageClose()
        {
            Global.currentFrame.NavigationService.GoBack();
        }
        public void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// 窗体最小化
        /// </summary>
        /// <param name="obj">指定窗体窗体</param>
        public void WindowMin(Object obj)
        {
            WindowTemplet win = (WindowTemplet)obj;
            win.WindowState = WindowState.Minimized;
        }


        #endregion // RequestClose [event]
    }
}
