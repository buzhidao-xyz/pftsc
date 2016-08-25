using PFTSDesktop.Command;
using PFTSDesktop.Model;
using PFTSDesktop.View.DeviceManager;
using PFTSModel;
using PFTSUITemplate.Controls;
using PFTSUITemplate.Element;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PFTSDesktop.ViewModel
{
    public class LockerManagerViewModel : WorkspaceViewModel,IDataErrorInfo
    {
         #region 私有变量
        private RelayCommand _editLockerDlgCommand;
        private RelayCommand _addLockerDlgCommand;
        private LockerModel _lockerModel;
        private LockerModel _lockerAddModel;
        private RelayCommand _editLockerCommand;
        private RelayCommand _addLockerCommand;
        private dev_lockers _dev_locker;
        private static LockerManagerViewModel instance;
        private DevLockerService lockerService;
        private RelayCommand _getLockerListCommand;
        private List<dev_lockers> _lockerList;
        private int type;
        #endregion

        public LockerManagerViewModel()
        {
            lockerService = new DevLockerService();
            _dev_locker = new dev_lockers();

            _lockerAddModel = new LockerModel();
            _lockerModel = new LockerModel();
        }

        public static LockerManagerViewModel GetInstance()
        {

            if (instance == null)
            {
                instance = new LockerManagerViewModel();
            }

            return instance;
        }

        #region command
        public ICommand AddLockerDlgCommand
        {
            get
            {
                if (_addLockerDlgCommand == null)
                {
                    _addLockerDlgCommand = new RelayCommand(
                        param => this.AddLockerDlg()
                        );
                }
                return _addLockerDlgCommand;
            }
        }

        public ICommand EditLockerDlgCommand
        {
            get
            {
                if (_editLockerDlgCommand == null)
                {
                    _editLockerDlgCommand = new RelayCommand(
                        param => this.EditLockerDlg()
                        );
                }
                return _editLockerDlgCommand;
            }
        }

        public ICommand EditLockerCommand
        {
            get
            {
                if (_editLockerCommand == null)
                {
                    _editLockerCommand = new RelayCommand(
                        new Action<Object>(this.EditLocker),
                        param => this.CanSave
                        );
                }
                return _editLockerCommand;
            }
        }

        public ICommand AddLockerCommand
        {
            get
            {
                if (_addLockerCommand == null)
                {
                    _addLockerCommand = new RelayCommand(
                        new Action<Object>(this.EditLocker),
                        param => this.AddCanSave
                        );
                }
                return _addLockerCommand;
            }
        }

        #endregion

        #region 属性
        public dev_lockers DevLocker
        {
            get
            {
                if (_dev_locker == null)
                {
                    _dev_locker = new dev_lockers();
                }
                return _dev_locker;
            }
            set
            {
                if (value == _dev_locker || value == null)
                    return;
                _dev_locker = value;
                _lockerModel.Id = _dev_locker.id;
                _lockerModel.Name = _dev_locker.name;
                _lockerModel.No = _dev_locker.no;

                base.OnPropertyChanged("DevLocker");
            }
        }

        public LockerModel GetLockerModel
        {
            get
            {
                return _lockerModel;
            }
            set
            {
                if (value == _lockerModel)
                    return;
                _lockerModel = value;
                base.OnPropertyChanged("GetLockerModel");
            }
        }

        public LockerModel LockerAddModel
        {
            get
            {
                return _lockerAddModel;
            }
            set
            {
                if (value == _lockerAddModel)
                    return;
                _lockerAddModel = value;
                base.OnPropertyChanged("LockerAddModel");
            }
        }

        public List<dev_lockers> LockerList
        {
            get
            {
                _lockerList = lockerService.GetAll();
                return _lockerList;
            }
            set
            {
                if (value == _lockerList)
                    return;
                _lockerList = value;
                base.OnPropertyChanged("LockerList");
            }
        }

        #endregion

        #region 方法
        private void AddLockerDlg()
        {
            type = 1;
            LockerAddModel = new LockerModel();
            LockerAddDlg dlg = new LockerAddDlg();
            dlg.ShowDialog();
        }

        private void EditLockerDlg()
        {
            type = 2;
            LockerEditDlg dlg = new LockerEditDlg();
            _lockerModel.OldName = DevLocker.name;
            _lockerModel.OldNo = DevLocker.no;
            dlg.ShowDialog();
        }

        private void EditLocker(Object obj)
        {
            bool result = false;

            if (type==1)
            {
                dev_lockers model = new dev_lockers();
                model.no = _lockerAddModel.No;
                model.name = _lockerAddModel.Name;
                model.create_time = DateTime.Now;
                result = lockerService.Insert(model);

            }
            else
            {
                _dev_locker.no = _lockerModel.No;
                _dev_locker.name = _lockerModel.Name;
                result = lockerService.ModifyLocker(_dev_locker);
            }
            if (result)
            {
                MessageWindow.Show("储物柜信息更新成功！", "系统提示");
                WindowTemplet window = (WindowTemplet)obj;
                window.Close();
                LockerList = lockerService.GetAll();
            }
            else
            {
                MessageWindow.Show("储物柜信息更新失败！", "错误");
            }
        }

        bool CanSave
        {
            get { return _lockerModel.IsValid; }
        }

        bool AddCanSave
        {
            get { return _lockerAddModel.IsValid; }
        }
        #endregion

      

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get
            {
                if (type == 1)
                    return (_lockerAddModel as IDataErrorInfo).Error;
                else
                    return (_lockerModel as IDataErrorInfo).Error;
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                string error = null;
                if (type == 1)
                    error = (_lockerAddModel as IDataErrorInfo)[propertyName];
                else
                    error = (_lockerModel as IDataErrorInfo)[propertyName];

                // Dirty the commands registered with CommandManager,
                // such as our Save command, so that they are queried
                // to see if they can execute now.
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }
        #endregion // IDataErrorInfo Members
    }

}
