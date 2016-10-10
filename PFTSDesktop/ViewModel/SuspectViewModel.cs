using PFTSDesktop.Command;
using PFTSDesktop.Model;
using PFTSDesktop.Properties;
using PFTSDesktop.View.Monitoring;
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
using System.Windows.Controls;
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
        private string inNo, inName,allNo,allName;
        private DateTime? inTimeStart, inTimeEnd,allTimeStart,allTimeEnd;
        private ContentControl _workSpace;
        private AllSuspectControl allSuspectControl;
        private InSuspectControl insuspectContrl;
        private RelayCommand _gatherCommand;
        private RelayCommand _friskCommand;
        private RelayCommand _videoCommand;
        private RelayCommand _trackCommand;
        private string _inSuspectName;
        private string _allSuspectName;
        private string _inSuspectNo;
        private string _allSuspectNo;
        private DateTime? _inTimeStart;
        private DateTime? _allTimeStart;
        private DateTime? _inTimeEnd;
        private DateTime? _allTimeEnd;
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

        public ContentControl Workspace
        {
            get
            {
                return _workSpace;
            }
            set
            {
                if (value == _workSpace)
                    return;
                _workSpace = value;
                base.OnPropertyChanged("Workspace");
            }
        }

        public string InSuspectName
        {
            get
            {
                return _inSuspectName;
            }
            set
            {
                if (value == _inSuspectName)
                    return;
                _inSuspectName = value;
                base.OnPropertyChanged("InSuspectName");
            }
        }

        public string AllSuspectName
        {
            get
            {
                return _allSuspectName;
            }
            set
            {
                if (value == _allSuspectName)
                    return;
                _allSuspectName = value;
                base.OnPropertyChanged("AllSuspectName");
            }
        }

        public string InSuspectNo
        {
            get
            {
                return _inSuspectNo;
            }
            set
            {
                if (value == _inSuspectNo)
                    return;
                _inSuspectNo = value;
                base.OnPropertyChanged("InSuspectNo");
            }
        }

        public string AllSuspectNo
        {
            get
            {
                return _allSuspectNo;
            }
            set
            {
                if (value == _allSuspectNo)
                    return;
                _allSuspectNo = value;
                base.OnPropertyChanged("AllSuspectNo");
            }
        }

        public DateTime? InTimeStart
        {
            get
            {
                return _inTimeStart;
            }
            set
            {
                if (value == _inTimeStart)
                    return;
                _inTimeStart = value;
                base.OnPropertyChanged("InTimeStart");
            }
        }
       
        public DateTime? AllTimeStart
        {
            get
            {
                return _allTimeStart;
            }
            set
            {
                if (value == _allTimeStart)
                    return;
                _allTimeStart = value;
                base.OnPropertyChanged("AllTimeStart");
            }
        }
        public DateTime? InTimeEnd
        {
            get
            {
                return _inTimeEnd;
            }
            set
            {
                if (value == _inTimeEnd)
                    return;
                _inTimeEnd = value;
                base.OnPropertyChanged("InTimeEnd");
            }
        }

        public DateTime? AllTimeEnd
        {
            get
            {
                return _allTimeEnd;
            }
            set
            {
                if (value == _allTimeEnd)
                    return;
                _allTimeEnd = value;
                base.OnPropertyChanged("AllTimeEnd");
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

        /// <summary>
        /// 嫌疑人采集事件
        /// </summary>
        public ICommand SuspectGatherCommand
        {
            get
            {
                if (_gatherCommand == null)
                {
                    _gatherCommand = new RelayCommand(
                     param => this.SuspectGather()
                    );
                }
                return _gatherCommand;
            }
        }

        /// <summary>
        /// 嫌疑人采集事件
        /// </summary>
        public ICommand SuspectFriskCommand
        {
            get
            {
                if (_friskCommand == null)
                {
                    _friskCommand = new RelayCommand(
                     param => this.SuspectFrisk()
                    );
                }
                return _friskCommand;
            }
        }
        /// <summary>
        /// 查看监控视频事件
        /// </summary>
        public ICommand SuspectVideoCommand
        {
            get
            {
                if (_videoCommand == null)
                {
                    _videoCommand = new RelayCommand(
                     param => this.SuspectVideo()
                    );
                }
                return _videoCommand;
            }
        }

        /// <summary>
        /// 查看历史轨迹事件
        /// </summary>
        public ICommand SuspectTrackCommand
        {
            get
            {
                if (_trackCommand == null)
                {
                    _trackCommand = new RelayCommand(
                     param => this.SuspectTrack()
                    );
                }
                return _trackCommand;
            }
        }

        /// <summary>
        /// 重置在监人员搜索条件
        /// </summary>
        public ICommand InResetCommand
        {
            get
            {
                return new RelayCommand(param => this.InReset());
            }
        }
        /// <summary>
        /// 重置所有嫌犯搜索条件
        /// </summary>
        public ICommand AllResetCommand
        {
            get
            {
                return new RelayCommand(param => this.AllReset());
            }
        }

        public ICommand SelectInSuspectCommand
        {
            get
            {
                return new RelayCommand(param => this.SelectInSuspect());
            }
        }

        public ICommand SelectAllSuspectCommand
        {
            get
            {
                return new RelayCommand(param => this.SelectAllSuspect());
            }
        }
        #endregion

        #region 方法
        public void SelectInSuspect()
        {
            inName = InSuspectName;
            inNo = InSuspectNo;
            inTimeStart = InTimeStart;
            inTimeEnd = InTimeEnd;
            initData();
        }

        public void SelectAllSuspect()
        {
            allName = AllSuspectName;
            allNo = AllSuspectNo;
            allTimeStart = AllTimeStart;
            allTimeEnd = AllTimeEnd;
            initData();
        }
        public void InReset()
        {
            InSuspectName = "";
            InSuspectNo = "";
            InTimeStart =null;
            InTimeEnd = null;
        }

        public void AllReset()
        {
            AllSuspectName = "";
            AllSuspectNo = "";
            AllTimeStart = null;
            AllTimeEnd =null;
        }
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
            SuspectModel = new Model.SuspectModel();
            OfficerInfo = new officer();
            DevVest = new view_vest_info();
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
                if (insuspectContrl == null)
                    insuspectContrl = new InSuspectControl(); ;
                Workspace = insuspectContrl;
                status = 0;

               
            }
            else
            {
                if (allSuspectControl == null)
                    allSuspectControl = new AllSuspectControl(); ;
                Workspace = allSuspectControl;
                status = null;
               
            }
            initData();
        }

        /// <summary>
        /// 嫌疑人采集操作
        /// </summary>
        public void SuspectGather()
        {
            bool result = _supectService.GatherSuspect(_selectedBtracker.id);
            if (result)
            {
                initData();
            }
            else
            {
                MessageWindow.Show("操作失败", "错误");
            }
        }
        /// <summary>
        /// 嫌疑人搜身
        /// </summary>
        public void SuspectFrisk()
        {
            bool result = _supectService.FriskSuspect(_selectedBtracker.id);
            if (result)
            {
                initData();
            }
            else
            {
                MessageWindow.Show("操作失败", "错误");
            }
        }

        /// <summary>
        /// 查看监控视频
        /// </summary>
        public void SuspectVideo()
        {
            PFTSModel.dev_camera camera = (new PFTSModel.Services.CameraPositionService()).GetByRoomId(SelectedBreacker.room_id.Value);
            if (camera == null)
            {
                MessageWindow.Show("未检测到该区域的摄像头信息", "系统提示");
                return;
            }
            VideoListWindow vw = new VideoListWindow(camera, SelectedBreacker.id);
            vw.Show();
        }
        /// <summary>
        /// 查看历史轨迹
        /// </summary>
        public void SuspectTrack()
        {
            btracker model = new btracker();
            model.id = SelectedBreacker.id;
            model.name = SelectedBreacker.name;
            model.room_id = SelectedBreacker.room_id;
            HistoricalTrackDlg dlg = new HistoricalTrackDlg(model);
            dlg.Show();
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
            if (status == null)
            {
                Btrackers = _supectService.GetPageByStatus(status, allName, allNo, allTimeStart, allTimeEnd, (PageIndex - 1) * PageSize, PageSize);
            }
            else
            {
                Btrackers = _supectService.GetPageByStatus(status, inName, inNo, inTimeStart, inTimeEnd, (PageIndex - 1) * PageSize, PageSize);
            }
        }

        private void initData()
        {
            if (status == null)
            {
                TotalCount = _supectService.GetCount(status, allName, allNo, allTimeStart, allTimeEnd);
                if (TotalCount == 0)
                {
                    Btrackers = new List<view_btracker_info>();
                }
                else
                {
                    Btrackers = _supectService.GetPageByStatus(status, allName, allNo, allTimeStart, allTimeEnd, (PageIndex - 1) * PageSize, PageSize);
                }
              
            }
            else
            {
                TotalCount = _supectService.GetCount(status, inName, inNo, inTimeStart, inTimeEnd);
                if (TotalCount == 0)
                {
                    Btrackers = new List<view_btracker_info>();
                }
                else
                {
                    Btrackers = _supectService.GetPageByStatus(status, inName, inNo, inTimeStart, inTimeEnd, (PageIndex - 1) * PageSize, PageSize);
                }
                
            }
        }
        #endregion
    }
}
