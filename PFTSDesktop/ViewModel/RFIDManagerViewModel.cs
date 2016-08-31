using PFTSDesktop.Command;
using PFTSDesktop.Model;
using PFTSDesktop.View.DeviceManager;
using PFTSModel;
using PFTSModel.Services;
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
    public class RFIDManagerViewModel :  WorkspaceViewModel, IDataErrorInfo
    {
        #region 私有变量
        private RelayCommand _ediRFIDDlgCommand;
        private RelayCommand _addRFIDDlgCommand;
        private RFIDModel _rfidModel;
        private RFIDModel _rfidAddModel;
        private RelayCommand _editRFIDCommand;
        private RelayCommand _addRFIDCommand;
        private view_rfid_info _dev_rfid;
        private static RFIDManagerViewModel instance;
        private DevRFIDService rfidService;
        private List<view_rfid_info> _rfidList;
        private int type;
  
        #endregion

        public RFIDManagerViewModel()
        {
            rfidService = new DevRFIDService();

            _dev_rfid = new view_rfid_info();

            _rfidAddModel = new RFIDModel();
            _rfidModel = new RFIDModel();
            initData();
        }

        public static RFIDManagerViewModel GetInstance()
        {

            if (instance == null)
            {
                instance = new RFIDManagerViewModel();
            }

            return instance;
        }

        #region command
        /// <summary>
        /// 打开添加框命令
        /// </summary>
        public ICommand AddRIFDDlgCommand
        {
            get
            {
                if (_addRFIDDlgCommand == null)
                {
                    _addRFIDDlgCommand = new RelayCommand(
                        param => this.AddRFIDDlg()
                        );
                }
                return _addRFIDDlgCommand;
            }
        }

        /// <summary>
        /// 打开编辑框命令
        /// </summary>
        public ICommand EditRFIDDlgCommand
        {
            get
            {
                if (_ediRFIDDlgCommand == null)
                {
                    _ediRFIDDlgCommand = new RelayCommand(
                        param => this.EditRFIDDlg()
                        );
                }
                return _ediRFIDDlgCommand;
            }
        }

        /// <summary>
        /// 编辑rfid天线命令
        /// </summary>
        public ICommand EditRFIDCommand
        {
            get
            {
                if (_editRFIDCommand == null)
                {
                    _editRFIDCommand = new RelayCommand(
                        new Action<Object>(this.EditRFID),
                        param => this.CanSave
                        );
                }
                return _editRFIDCommand;
            }
        }

        /// <summary>
        /// 添加rfid天线命令
        /// </summary>
        public ICommand AddRFIDCommand
        {
            get
            {
                if (_addRFIDCommand == null)
                {
                    _addRFIDCommand = new RelayCommand(
                        new Action<Object>(this.EditRFID),
                        param => this.AddCanSave
                        );
                }
                return _addRFIDCommand;
            }
        }

        #endregion

        #region 属性

        

        /// <summary>
        /// rfid天线实体
        /// </summary>
        public view_rfid_info DevRFID
        {
            get
            {
                if (_dev_rfid == null)
                {
                    _dev_rfid = new view_rfid_info();
                }
                return _dev_rfid;
            }
            set
            {
                if (value == _dev_rfid || value == null)
                    return;
                _dev_rfid = value;
                _rfidModel.Id = _dev_rfid.id;
                _rfidModel.Name = _dev_rfid.name;
                _rfidModel.No = _dev_rfid.no;

                base.OnPropertyChanged("DevRFID");
            }
        }

        /// <summary>
        /// 修改用的rifd天线实体
        /// </summary>
        public RFIDModel GetRFIDModel
        {
            get
            {
                return _rfidModel;
            }
            set
            {
                if (value == _rfidModel)
                    return;
                _rfidModel = value;
                base.OnPropertyChanged("GetRFIDModel");
            }
        }

        /// <summary>
        /// 添加rfid天线的实体
        /// </summary>
        public RFIDModel RFIDAddModel
        {
            get
            {
                return _rfidAddModel;
            }
            set
            {
                if (value == _rfidAddModel)
                    return;
                _rfidAddModel = value;
                base.OnPropertyChanged("RFIDAddModel");
            }
        }

        /// <summary>
        /// 所有rfid天线列表
        /// </summary>
        public List<view_rfid_info> RFIDList
        {
            get
            {
                return _rfidList;
            }
            set
            {
                if (value == _rfidList)
                    return;
                _rfidList = value;
                base.OnPropertyChanged("RFIDList");
            }
        }

        #endregion

        #region 方法
        private void AddRFIDDlg()
        {
            type = 1;
            RFIDAddModel = new RFIDModel();
            RFIDAddDlg dlg = new RFIDAddDlg();
            dlg.ShowDialog();
        }

        private void EditRFIDDlg()
        {
            type = 2;
            RFIDEditDlg dlg = new RFIDEditDlg();
            _rfidModel.OldName = DevRFID.name;
            _rfidModel.OldNo = DevRFID.no;
            dlg.ShowDialog();
        }

        private void EditRFID(Object obj)
        {
            bool result = false;

            if (type == 1)
            {
                dev_rfid model = new dev_rfid();
                model.no = _rfidAddModel.No;
                model.name = _rfidAddModel.Name;
                model.create_time = DateTime.Now;
               
                result = rfidService.Insert(model);

            }
            else
            {
                _dev_rfid.no = _rfidModel.No;
                _dev_rfid.name = _rfidModel.Name;
                
                result = rfidService.ModifyLocker(_dev_rfid);
            }
            if (result)
            {
                MessageWindow.Show("RFID天线信息更新成功！", "系统提示");
                WindowTemplet window = (WindowTemplet)obj;
                window.Close();
                initData();
            }
            else
            {
                MessageWindow.Show("RFID天线信息更新失败！", "错误");
            }
        }

        bool CanSave
        {
            get { return _rfidModel.IsValid; }
        }

        bool AddCanSave
        {
            get { return _rfidAddModel.IsValid; }
        }
        #endregion



        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get
            {
                if (type == 1)
                    return (_rfidAddModel as IDataErrorInfo).Error;
                else
                    return (_rfidModel as IDataErrorInfo).Error;
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                string error = null;
                if (type == 1)
                    error = (_rfidAddModel as IDataErrorInfo)[propertyName];
                else
                    error = (_rfidModel as IDataErrorInfo)[propertyName];

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
            RFIDList = rfidService.GetPageByStatus(null, (PageIndex - 1) * PageSize, PageSize);
        }

        private void initData()
        {
            TotalCount = rfidService.GetCount(null);
            if (TotalCount == 0)
            {
                RFIDList = new List<view_rfid_info>();
                PageIndex = 0;
            }
            else
            {
                RFIDList = rfidService.GetPageByStatus(null, (PageIndex - 1) * PageSize, PageSize);
            }
        }
        #endregion
    }
}
