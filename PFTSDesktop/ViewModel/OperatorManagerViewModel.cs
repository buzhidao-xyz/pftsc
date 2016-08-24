using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFTSModel;
using PFTSDesktop.Command;
using System.Windows.Input;
using PFTSUITemplate.Controls;
using System.Windows.Controls;
using System.Windows;
using PFTSTools;

namespace PFTSDesktop.ViewModel
{
    class OperatorManagerViewModel : WorkspaceViewModel
    {
        //操作员serviceobject
        private OperatorService OperatorService;

        private RelayCommand OperatorCommand;

        //操作员列表
        private List<@operator> OperatorList;
        //操作员信息
        private @operator OperatorInfo;

        public OperatorManagerViewModel()
        {
            OperatorService = new OperatorService();
        }

        /// <summary>
        /// 获取操作员列表
        /// </summary>
        public List<@operator> GetOperatorList
        {
            get {
                if (OperatorList == null) OperatorList = OperatorService.GetOperatorList();

                return OperatorList;
            }
            set
            {
                if (value == OperatorList)
                    return;

                OperatorList = value;

                base.OnPropertyChanged("GetOperatorList");
            }
        }

        /// <summary>
        /// 获取操作员信息
        /// </summary>
        public @operator GetOperatorInfo
        {
            get {
                if (OperatorInfo == null) OperatorInfo = new @operator();

                return OperatorInfo;
            }
            set
            {
                if (value == OperatorInfo)
                    return;

                OperatorInfo = value;

                base.OnPropertyChanged("GetOperatorInfo");
            }
        }

        /// <summary>
        /// 操作员管理命令
        /// </summary>
        public ICommand OperatorManageCommand
        {
            get
            {
                if (OperatorCommand == null)
                {
                    OperatorCommand = new RelayCommand(
                        new Action<Object>(this.OperatorManage)
                        );
                }
                return OperatorCommand;
            }
        }

        /// <summary>
        /// 操作员管理操作
        /// </summary>
        public void OperatorManage(Object obj)
        {
            Button btn = (Button)obj;
            Global.currentFrame.Source = new Uri(btn.Tag.ToString(), UriKind.Relative);
        }

        /// <summary>
        /// 新增操作员命令
        /// </summary>
        public ICommand OperatorNewCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.OperatorNew)
                    );
            }
        }

        /// <summary>
        /// 新增操作员
        /// </summary>
        public void OperatorNew(Object obj)
        {
            Button btn = (Button)obj;
            Global.currentFrame.Source = new Uri(btn.Tag.ToString(), UriKind.Relative);
        }

        /// <summary>
        /// 新增操作员保存命令
        /// </summary>
        public ICommand OperatorNewSaveCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.OperatorNewSave)
                    );
            }
        }

        /// <summary>
        /// 账号
        /// </summary>
        private string account
        {
            get
            {
                if (OperatorInfo == null) OperatorInfo = GetOperatorInfo;
                var account = OperatorInfo.account.Trim();

                //检查是否为空
                if (account == null)
                {
                    MessageBox.Show("请填写账号！");

                    return null;
                }

                //检查account是否已存在
                if (OperatorService.GetByAccount(OperatorInfo.account) != null)
                {
                    MessageBox.Show("账号 " + OperatorInfo.account + " 已存在！");

                    return null;
                }

                return account;
            }
        }

        /// <summary>
        /// 密码
        /// </summary>
        private string password
        {
            get
            {
                if (OperatorInfo == null) OperatorInfo = GetOperatorInfo;
                var password = OperatorInfo.password.Trim();

                //检查是否为空
                if (password == null)
                {
                    MessageBox.Show("请填写密码！");

                    return null;
                }

                return password;
            }
        }

        /// <summary>
        /// 昵称
        /// </summary>
        private string name
        {
            get
            {
                if (OperatorInfo == null) OperatorInfo = GetOperatorInfo;
                var name = OperatorInfo.name.Trim();

                //检查是否为空
                if (name == null)
                {
                    MessageBox.Show("请填写昵称！");

                    return null;
                }

                return name;
            }
        }

        /// <summary>
        /// 新增操作员保存
        /// </summary>
        /// <param name="obj"></param>
        public void OperatorNewSave(Object obj)
        {
            OperatorInfo = GetOperatorInfo;

            OperatorInfo.account = account;
            if (OperatorInfo.account == null) return;

            OperatorInfo.password = password;
            if (OperatorInfo.password == null) return;

            OperatorInfo.name = name;
            if (OperatorInfo.name == null) return;
            
            //处理数据//
            //密码MD5加密
            OperatorInfo.password = MD5Tool.GetEncryptCode(OperatorInfo.password);

            bool Result = OperatorService.Insert(OperatorInfo);
            if (Result)
            {
                MessageBox.Show("保存成功！");

                Button btn = (Button)obj;
                Global.currentFrame.NavigationService.GoBack();
            } else
            {
                MessageBox.Show("保存失败！");
            }
        }

        /// <summary>
        /// 编辑操作员命令
        /// </summary>
        public ICommand OperatorUpCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.OperatorUp)
                    );
            }
        }

        /// <summary>
        /// 编辑操作员保存命令
        /// </summary>
        private void OperatorUp(Object obj)
        {
            MessageBox.Show(GetOperatorInfo.account);

            Button btn = (Button)obj;
            Global.currentFrame.Source = new Uri(btn.Tag.ToString(), UriKind.Relative);
        }

        public ICommand OperatorUpSaveCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.OperatorUpSave)
                    );
            }
        }

        /// <summary>
        /// 编辑操作员保存
        /// </summary>
        private void OperatorUpSave(Object obj)
        {
            OperatorInfo = GetOperatorInfo;
            
            OperatorInfo.password = password;
            if (OperatorInfo.password == null) return;

            OperatorInfo.name = name;
            if (OperatorInfo.name == null) return;

            //处理数据//
            //密码MD5加密
            OperatorInfo.password = MD5Tool.GetEncryptCode(OperatorInfo.password);

            bool Result = OperatorService.Insert(OperatorInfo);
            if (Result)
            {
                MessageBox.Show("保存成功！");

                Button btn = (Button)obj;
                Global.currentFrame.NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("保存失败！");
            }
        }
    }
}
