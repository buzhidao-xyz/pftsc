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
        private dev_rfid _dev_rfid;
        private static RFIDManagerViewModel instance;
        private DevRFIDService rfidService;
        private RelayCommand _getRFIDListCommand;
        private List<dev_rfid> _rfidList;
        private int type;
        #endregion

        public RFIDManagerViewModel()
        {
            rfidService = new DevRFIDService();
            _dev_rfid = new dev_rfid();

            _rfidAddModel = new RFIDModel();
            _rfidModel = new RFIDModel();
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
        public dev_rfid DevRFID
        {
            get
            {
                if (_dev_rfid == null)
                {
                    _dev_rfid = new dev_rfid();
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

        public List<dev_rfid> RFIDList
        {
            get
            {
                _rfidList = rfidService.GetAll();
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
                RFIDList = rfidService.GetAll();
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
    }
}
