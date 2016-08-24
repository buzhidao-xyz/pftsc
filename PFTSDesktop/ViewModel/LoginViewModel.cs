using PFTSDesktop.Command;
using PFTSDesktop.Model;
using PFTSDesktop.Properties;
using PFTSModel;
using PFTSTools;
using PFTSUITemplate.Controls;
using PFTSUITemplate.Element;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PFTSHwCtrl;

namespace PFTSDesktop.ViewModel
{
    public class LoginViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        private readonly OperatorModel operatorModel;
        private RelayCommand logionCommand;
        private OperatorService operatorService;

        public LoginViewModel() {
            operatorModel = new OperatorModel();
            operatorService = new OperatorService();
            // TODO: test
            operatorModel.Account = "admin";
            operatorModel.Password = "123456";
        }

        #region 人员表属性

        public String Account {
            get { return operatorModel.Account; }
            set
            {
                if (value == operatorModel.Account)
                    return;

                operatorModel.Account = value;

                base.OnPropertyChanged("Account");
            }
        }

        public String Password
        {
            get { return operatorModel.Password; }
            set
            {
                if (value == operatorModel.Password)
                    return;

                operatorModel.Password = value;

                base.OnPropertyChanged("Password");
            }
        }

        #endregion

        public ICommand LoginCommand
        {
            get
            {
                if (logionCommand == null)
                {
                    logionCommand = new RelayCommand(
                        new Action<Object>(this.Login)
                        );
                }
                return logionCommand;
            }
        }

        public void Login(Object obj)
        {
            var proxy = new PFTSVideoProxy("192.168.10.164", 8000, "admin", "Gt123456");
            proxy.Login();
            proxy.StartRecord("D:\\aa.mp4");

            if (!operatorModel.IsValid)
                throw new InvalidOperationException(Resources.LoginViewModel_Exception_CannotLogin);
            //TODO 登录
            @operator opt = operatorService.GetByAccount(Account);
            if (opt == null || opt.password != MD5Tool.GetEncryptCode(Password))
            {
                MessageWindow.Show("账号或者密码不正确！", "系统提示", MessageWindowButton.OK, MessageWindowIcon.Error);
                return;
            }
            WindowTemplet win = (WindowTemplet)obj;
            MainWindow main = new MainWindow();
            var viewModel = new MainWindowViewModel();
            Global.currentFrame = main.PageContext;
            main.DataContext = viewModel;
            win.Close();
            main.Show();

        }

        bool CanLogin {
            get { return operatorModel.IsValid; }
        }

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get { return (operatorModel as IDataErrorInfo).Error; }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                string error = null;


                error = (operatorModel as IDataErrorInfo)[propertyName];
                

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
