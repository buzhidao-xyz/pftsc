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

        public ICommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(new Action<Object>(this.mainWindowClose));
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

        public void mainWindowClose(Object obj)
        {
            WindowTemplet win = (WindowTemplet)obj;
            System.Environment.Exit(0);
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

        #region 分页属性
        private int _totalPage = 20;
        private int _pageSize = 20;
        private int _pageIndex = 1;
        private int _totalCount;

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage
        {
            get
            {
                return _totalPage;
            }
            set
            {
                _totalPage = value;
                base.OnPropertyChanged("TotalPage");
            }
        }

        /// <summary>
        /// 每页显示记录数
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
                base.OnPropertyChanged("PageSize");
            }
        }

        /// <summary>
        /// 需要加载的页码
        /// </summary>
        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                _pageIndex = value;

                base.OnPropertyChanged("PageIndex");
            }
        }

        /// <summary>
        /// 记录总数
        /// </summary>
        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                _totalCount = value;

                base.OnPropertyChanged("TotalCount");
            }
        }
        #endregion
    }
}
