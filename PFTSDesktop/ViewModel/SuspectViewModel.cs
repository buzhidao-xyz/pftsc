using PFTSDesktop.Command;
using PFTSDesktop.Model;
using PFTSDesktop.Properties;
using PFTSDesktop.View.SuspectManager;
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
    public class SuspectViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        #region 私有变量
        private RelayCommand _addSupcetCommand;
        private RelayCommand _getSuspectsCommand;
        private RelayCommand _transferCommand;
        private ButtonTemplet _preBtn;
        private SuspectModel _suspectModel;
        private List<officer> _officers;
        private List<view_vest_info> _devVests;
        private OfficerService _officerService;
        private DevVestService _devVestService;
        private DevLockerService _devLockerService;
        private List<view_locker_info> _devLockers;
        private view_locker_info _devLocker;
        private officer _officer;
        private view_vest_info _devVert;
        private PFTSModel.Services.BTrackerService _supectService;
        private List<view_btracker_info> _btrackers;
        private string[] _sexOpetions;
        private static SuspectViewModel instance;
        private view_btracker_info _selectedBtracker;
        private bool _recover;
        private string _remark;
        private int? status = 0;
        #endregion

        public SuspectViewModel()
        {
            //suspectModel = new SuspectModel();
            _officerService = new OfficerService();
            _devVestService = new DevVestService();
            _devLockerService = new DevLockerService();
            _supectService = new PFTSModel.Services.BTrackerService();

            _officer = new officer();
            _devVert = new view_vest_info();
            _devLocker = new view_locker_info();
            initData();
        }

        public static SuspectViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new SuspectViewModel();
            }

            return instance;
        }

        #region 属性
        /// <summary>
        /// 嫌疑犯实体
        /// </summary>
        public SuspectModel SuspectModel
        {
            get
            {
                if (_suspectModel == null)
                {
                    _suspectModel = new SuspectModel();
                    _suspectModel.Sex = "男";
                }
                return _suspectModel;
            }
            set
            {
                if (value == _suspectModel)
                    return;
                _suspectModel = value;
                base.OnPropertyChanged("SuspectModel");
            }
        }
        /// <summary>
        /// 物品是否已取出
        /// </summary>
        public bool Recover
        {
            get
            {
                return _recover;
            }
            set
            {
                _recover = value;
                base.OnPropertyChanged("Recover");
            }
        }

        /// <summary>
        /// 警察列表
        /// </summary>
        public List<officer> Officers
        {
            get
            {
                if (_officers == null)
                    _officers = _officerService.GetAll();
                return _officers;
            }
            set
            {
                if (value == _officers)
                    return;
                _officers = value;
                base.OnPropertyChanged("Officers");
            }
        }

        /// <summary>
        /// 马甲列表
        /// </summary>
        public List<view_vest_info> DevVests
        {
            get
            {
                if (_devVests == null)
                    _devVests = _devVestService.GetVestByStatus(false);
                return _devVests;
            }
            set
            {
                if (value == _devVests)
                    return;
                _devVests = value;
                base.OnPropertyChanged("DevVests");
            }
        }

        /// <summary>
        /// 储物柜列表
        /// </summary>
        public List<view_locker_info> DevLockers
        {
            get
            {
                if (_devLockers == null)
                    _devLockers = _devLockerService.GetLockersByStatus(false);
                return _devLockers;
            }
            set
            {
                if (value == _devLockers)
                    return;
                _devLockers = value;
                base.OnPropertyChanged("DevLockers");
            }
        }
        /// <summary>
        /// 储物柜实体
        /// </summary>
        public view_locker_info DevLocker
        {
            get
            {
                return _devLocker;
            }
            set
            {
                if (value == _devLocker || value == null)
                    return;
                _devLocker = value;
                _suspectModel.LockerId = _devLocker.id;
                base.OnPropertyChanged("DevLocker");
            }
        }

        /// <summary>
        /// 警察实体
        /// </summary>
        public officer OfficerInfo
        {
            get
            {
                return _officer;
            }
            set
            {
                if (value == _officer || value == null)
                    return;
                _officer = value;
                _suspectModel.OfficerId = _officer.id;
                base.OnPropertyChanged("OfficerInfo");
            }
        }

        /// <summary>
        /// 马甲实体
        /// </summary>
        public view_vest_info DevVest
        {
            get
            {
                return _devVert;
            }
            set
            {
                if (value == _devVert || value == null)
                    return;
                _devVert = value;
                _suspectModel.VestId = _devVert.id;
                base.OnPropertyChanged("DevVest");
            }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public string[] SexOptions
        {
            get
            {
                if (_sexOpetions == null)
                {
                    _sexOpetions = new string[]
                    {
                       "男",
                       "女"
                    };
                }
                return _sexOpetions;
            }
        }


        /// <summary>
        /// 嫌疑犯列表
        /// </summary>
        public List<view_btracker_info> Btrackers
        {
            get
            {
                return _btrackers;
            }
            set
            {
                if (value == _btrackers)
                    return;
                _btrackers = value;
                base.OnPropertyChanged("Btrackers");
            }
        }

        /// <summary>
        /// 选中的嫌疑犯
        /// </summary>
        public view_btracker_info SelectedBreacker
        {
            get
            {
                return _selectedBtracker;
            }
            set
            {
                if (value == _selectedBtracker || value == null)
                    return;
                _selectedBtracker = value;
                base.OnPropertyChanged("SelectedBreacker");
            }
        }

        /// <summary>
        /// 转移备注
        /// </summary>
        public string Remark
        {
            get
            {
                return _remark;
            }
            set
            {
                if (value == _remark)
                    return;
                _remark = value;
                base.OnPropertyChanged("Remark");
            }

        }
        #endregion

        #region command
        /// <summary>
        /// 添加嫌疑犯页面跳转事件
        /// </summary>
        public ICommand AddSupcetPageCommand
        {
            get
            {
                if (_addSupcetCommand == null)
                {
                    _addSupcetCommand = new RelayCommand(
                        new Action<Object>(this.AddSupectPage)
                        );
                }
                return _addSupcetCommand;
            }
        }

        /// <summary>
        /// 查询嫌疑犯列表事件
        /// </summary>
        public ICommand GetSuspectsCommand
        {
            get
            {
                if (_getSuspectsCommand == null)
                {
                    _getSuspectsCommand = new RelayCommand(
                        new Action<Object>(this.GetSuspects)
                        );
                }
                return _getSuspectsCommand;
            }
        }

        /// <summary>
        /// 添加嫌疑人事件
        /// </summary>
        public ICommand AddSupectInfoCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.AddSupectInfo),
                    param => this.CanSave
                    );
            }
        }

        /// <summary>
        /// 物品清单事件
        /// </summary>
        public ICommand CheckGoodsCommand
        {

            get
            {
                return new RelayCommand(param => this.CheckGoods());
            }
        }

        /// <summary>
        /// 人员转移窗体打开事件
        /// </summary>
        /// 
        public ICommand SuspectTransferDlgCommand
        {
            get
            {
                if (_transferCommand == null)
                {
                    _transferCommand = new RelayCommand(
                         new Action<Object>(this.SuspectTransferDlg)
                        );
                }
                return _transferCommand;
            }
        }

        /// <summary>
        /// 嫌疑犯转移事件
        /// </summary>
        public ICommand SuspectTransferCommand
        {
            get
            {
                return new RelayCommand(
                     new Action<Object>(this.SuspectTransfer)
                    );
            }
        }
        #endregion

        #region 方法

        public void SuspectTransferDlg(Object obj)
        {
            Remark = _selectedBtracker.remark;
            Recover = SelectedBreacker.recover == null ? false : (bool)SelectedBreacker.recover;
            SuspectTransferDlg dlg = new SuspectTransferDlg();
            dlg.ShowDialog();
        }
        public void SuspectTransfer(Object obj)
        {
            bool result = _supectService.TransferSuspect(_selectedBtracker.id, _recover, Remark);
            if (result)
            {
                MessageWindow.Show("嫌疑人转移成功", "系统提示");
                WindowTemplet window = (WindowTemplet)obj;
                window.Close();
                initData();
            }
            else
            {
                MessageWindow.Show("嫌疑人转移失败", "错误");
            }
        }
        public void CheckGoods()
        {
            CheckGoodsDlg dlg = new CheckGoodsDlg();
            dlg.ShowDialog();
        }
        public void AddSupectPage(Object obj)
        {
            AddSuspectDlg dlg = new AddSuspectDlg();
            dlg.ShowDialog();
            //ButtonEX btn = (ButtonEX)obj;
            //Global.currentFrame.Source = new Uri(btn.Tag.ToString(), UriKind.Relative);
        }

        public void AddSupectInfo(Object obj)
        {
            btracker btk = new btracker();
            btk.no = _suspectModel.No;
            btk.name = _suspectModel.Name;
            btk.sex = _suspectModel.Sex;
            btk.number = _suspectModel.Number;
            btk.officer_id = _suspectModel.OfficerId;
            btk.vest_id = _suspectModel.VestId;
            btk.locker_id = _suspectModel.LockerId;
            btk.private_goods = _suspectModel.PirvateGoods;
            var now = DateTime.Now;
            btk.in_time = now;
            btk.in_room_time = now;
            btk.room_id = 1;

            bool result = _supectService.Insert(btk);
            if (result)
            {
                MessageWindow.Show("嫌疑人添加成功", "系统提示");
                WindowTemplet window = (WindowTemplet)obj;
                window.Close();
                initData();
            }
            else
            {
                MessageWindow.Show("嫌疑人添加失败", "错误");
            }

        }

        bool CanSave
        {
            get { return String.IsNullOrEmpty(this.ValidateDevVest()) && String.IsNullOrEmpty(this.ValidateOfficer()) && _suspectModel.IsValid; }
        }

        public void GetSuspects(Object obj)
        {
             ButtonTemplet btn = (ButtonTemplet)obj;
            if (_preBtn != null)
            {
                if (btn == _preBtn)
                    return;
                _preBtn.SelectEd = false;
            }
            _preBtn = btn;
            btn.SelectEd = true;
            if (btn.Tag.ToString() == "0")
            {
                status = 0;
            }
            else
            {
                status = null;
            }
            initData();
        }
        #endregion
        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get { return (_suspectModel as IDataErrorInfo).Error; }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                string error = null;

                if (propertyName == "OfficerInfo")
                {
                    error = this.ValidateOfficer();
                }
                else if (propertyName == "DevVest")
                {
                    error = this.ValidateDevVest();
                }
                else
                {
                    error = (_suspectModel as IDataErrorInfo)[propertyName];

                }
                // Dirty the commands registered with CommandManager,
                // such as our Save command, so that they are queried
                // to see if they can execute now.
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }
        string ValidateDevVest()
        {
            if (this.DevVest != null && this.DevVest.id != 0)
                return null;

            return Resources.Suspect_Error_MissVest;
        }
        string ValidateOfficer()
        {
            if (this.OfficerInfo != null && this.OfficerInfo.id != 0)
                return null;

            return Resources.Suspect_Error_MissOfficer;
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
            Btrackers = _supectService.GetPageByStatus(status, (PageIndex - 1) * PageSize, PageSize);
        }

        private void initData()
        {
            TotalCount = _supectService.GetCount(status);
            if (TotalCount == 0)
            {
                Btrackers = new List<view_btracker_info>();
                PageIndex = 0;
            }
            else
            {
                Btrackers = _supectService.GetPageByStatus(status, (PageIndex - 1) * PageSize, PageSize);
            }
        }
        #endregion
    }
}
