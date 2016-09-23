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
    public class VestManagerViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        #region 私有变量
        private RelayCommand _editVestDlgCommand;
        private RelayCommand _addVestDlgCommand;
        private VestModel _vestModel;
        private VestModel _vestAddModel;
        private RelayCommand _editVestCommand;
        private RelayCommand _addVestCommand;
        private view_vest_info _dev_vest;
        private static VestManagerViewModel instance;
        private DevVestService vestService;
        private List<view_vest_info> _vestList;
        private int type;
        #endregion

        public VestManagerViewModel()
        {
            vestService = new DevVestService();
            _dev_vest = new view_vest_info();

            _vestAddModel = new VestModel();
            _vestModel = new VestModel();
        }


        public static VestManagerViewModel GetInstance()
        {

            if (instance == null)
            {
                instance = new VestManagerViewModel();
            }

            return instance;
        }

        #region command
        public ICommand AddVestDlgCommand
        {
            get
            {
                if (_addVestDlgCommand == null)
                {
                    _addVestDlgCommand = new RelayCommand(
                        param => this.AddVestDlg()
                        );
                }
                return _addVestDlgCommand;
            }
        }

        public ICommand EditVestDlgCommand
        {
            get
            {
                if (_editVestDlgCommand == null)
                {
                    _editVestDlgCommand = new RelayCommand(
                        param => this.EditVestDlg()
                        );
                }
                return _editVestDlgCommand;
            }
        }

        public ICommand EditVestCommand
        {
            get
            {
                if (_editVestCommand == null)
                {
                    _editVestCommand = new RelayCommand(
                        new Action<Object>(this.EditVest),
                        param => this.CanSave
                        );
                }
                return _editVestCommand;
            }
        }

        public ICommand AddVestCommand
        {
            get
            {
                if (_addVestCommand == null)
                {
                    _addVestCommand = new RelayCommand(
                        new Action<Object>(this.EditVest),
                        param => this.AddCanSave
                        );
                }
                return _addVestCommand;
            }
        }

        #endregion

        #region 方法
        private void AddVestDlg()
        {
            type = 1;
            VestAddModel = new VestModel();
            VestAddDlg dlg = new VestAddDlg();
            dlg.ShowDialog();
        }

        private void EditVestDlg()
        {
            type = 2;
            VestEditDlg dlg = new VestEditDlg();
            _vestModel.OldName = DevVest.name;
            _vestModel.OldNoLeft = DevVest.no_left;
            _vestModel.OldNoRight = DevVest.no_right;
            dlg.ShowDialog();
        }

        private void EditVest(Object obj)
        {
            bool result = false;

            if (type == 1)
            {
                dev_vest model = new dev_vest();
                model.no_left = _vestAddModel.NoLeft;
                model.no_right = _vestAddModel.NoRight;
                model.name = _vestAddModel.Name;
                model.create_time = DateTime.Now;
                result = vestService.Insert(model);

            }
            else
            {
                _dev_vest.no_left = _vestModel.NoLeft;
                _dev_vest.no_right = _vestModel.NoRight;
                _dev_vest.name = _vestModel.Name;
                result = vestService.ModifyVest(_dev_vest);
            }
            if (result)
            {
                MessageWindow.Show("马甲信息更新成功！", "系统提示");
                WindowTemplet window = (WindowTemplet)obj;
                window.Close();
                initData();
            }
            else
            {
                MessageWindow.Show("马甲信息更新失败！", "错误");
            }
        }

        bool CanSave
        {
            get { return _vestModel.IsValid; }
        }

        bool AddCanSave
        {
            get { return _vestAddModel.IsValid; }
        }
        #endregion

        #region 属性
        public view_vest_info DevVest
        {
            get
            {
                if (_dev_vest == null)
                {
                    _dev_vest = new view_vest_info();
                }
                return _dev_vest;
            }
            set
            {
                if (value == _dev_vest || value == null)
                    return;
                _dev_vest = value;
                _vestModel.Id = _dev_vest.id;
                _vestModel.Name = _dev_vest.name;
                _vestModel.NoLeft = _dev_vest.no_left;
                _vestModel.NoRight = _dev_vest.no_right;
                base.OnPropertyChanged("DevVest");
            }
        }

        public VestModel VestModel
        {
            get
            {
                return _vestModel;
            }
            set
            {
                if (value == _vestModel)
                    return;
                _vestModel = value;
                base.OnPropertyChanged("VestModel");
            }
        }

        public VestModel VestAddModel
        {
            get
            {
                return _vestAddModel;
            }
            set
            {
                if (value == _vestAddModel)
                    return;
                _vestAddModel = value;
                base.OnPropertyChanged("VestAddModel");
            }
        }

        public List<view_vest_info> VestList
        {
            get
            {
                TotalCount = vestService.GetVestCount(null);
                if (TotalCount == 0)
                {
                    _vestList = new List<view_vest_info>();
                }
                else
                {
                    _vestList = vestService.GetPageVestByStatus(null, (PageIndex - 1) * PageSize, PageSize);
                }
                return _vestList;
            }
            set
            {
                if (value == _vestList)
                    return;
                _vestList = value;
                base.OnPropertyChanged("VestList");
            }
        }

        #endregion

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get
            {
                if (type == 1)
                    return (_vestAddModel as IDataErrorInfo).Error;
                else
                    return (_vestModel as IDataErrorInfo).Error;
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                string error = null;
                if (type == 1)
                    error = (_vestAddModel as IDataErrorInfo)[propertyName];
                else
                    error = (_vestModel as IDataErrorInfo)[propertyName];

                // Dirty the commands registered with CommandManager,
                // such as our Save command, so that they are queried
                // to see if they can execute now.
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }
        #endregion // IDataErrorInfo Members

        #region 分页相关事件
      
        private RelayCommand _nextPageSearchCommand;
        public ICommand NextPageSearchCommand
        {
            get
            {
                if (_nextPageSearchCommand == null)
                {
                    _nextPageSearchCommand = new RelayCommand(
                            param => this.QueryData()
                            );
                }
                return _nextPageSearchCommand;
            }
        }
        private void QueryData()
        {
            VestList = vestService.GetPageVestByStatus(null, (PageIndex - 1) * PageSize, PageSize);
        }

        private void initData()
        {
            TotalCount = vestService.GetVestCount(null);
            if (TotalCount == 0)
            {
                VestList = new List<view_vest_info>();
            }
            else
            {
                VestList = vestService.GetPageVestByStatus(null, (PageIndex - 1) * PageSize, PageSize);
            }
        }
        #endregion
    }
}
