using PFTSDesktop.Command;
using PFTSUITemplate.Controls;
using PFTSUITemplate.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PFTSDesktop.ViewModel
{
    public class DeviceManagerViewModel:WorkspaceViewModel
    {
        private ICommand _functionChangeCommand;
        private WorkspaceViewModel _workSpace;
        private ButtonTemplet _preBtn;
        private String _functionName;
        #region command
        public ICommand FunctionChangeCommand
        {
            get
            { 
                if (_functionChangeCommand == null)
                {
                    _functionChangeCommand = new RelayCommand(
                        new Action<Object>(this.ChangeCommand)
                        );
                }
                return _functionChangeCommand;
            }
        }
        #endregion

        private void ChangeCommand(Object obj)
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
            FunctionName = btn.Content.ToString();
            switch (btn.Tag.ToString())
            {
                case "1":
                    Workspace =  VestManagerViewModel.GetInstance();
                    break;
                case "2":
                    Workspace =  LockerManagerViewModel.GetInstance();
                    break;
                case "3":
                    Workspace = new RFIDManagerViewModel();
                    break;
                case "4":
                    Workspace = new CameraManagerViewModel();
                    break;
            }
        }

        public WorkspaceViewModel Workspace
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

        public String FunctionName
        {
            get
            {
                return _functionName;
            }
            set
            {
                if (value == _functionName)
                    return;
                _functionName = value;
                base.OnPropertyChanged("FunctionName");
            }
        }
    }
}
