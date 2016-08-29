using PFTSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFTSTools;
using PFTSDesktop.Command;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;

namespace PFTSDesktop.ViewModel
{
    class PoliceManagerViewModel : WorkspaceViewModel
    {
        private static PoliceManagerViewModel instance;

        private OfficerService PoliceService;

        //警员列表
        private List<@officer> PoliceList;
        //警员信息
        private @officer PoliceInfo;

        //性别枚举
        public enum Sex
        {
            男,
            女
        }
        private Sex _sex;

        public PoliceManagerViewModel()
        {
            PoliceService = new OfficerService();

            PoliceInfo = new @officer();
        }

        public static PoliceManagerViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new PoliceManagerViewModel();
            }

            return instance;
        }

        /// <summary>
        /// 获取警员列表
        /// </summary>
        public List<@officer> GetPoliceList
        {
            get
            {
                PoliceList = PoliceService.GetOfficerList();

                return PoliceList;
            }
            set
            {
                if (value == PoliceList)
                    return;

                PoliceList = value;

                base.OnPropertyChanged("GetPoliceList");
            }
        }

        /// <summary>
        /// 获取警员信息
        /// </summary>
        public @officer GetPoliceInfo
        {
            get
            {
                return PoliceInfo;
            }
            set
            {
                if (value == PoliceInfo)
                    return;

                PoliceInfo = value;

                base.OnPropertyChanged("GetPoliceInfo");
            }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public Sex GetSex
        {
            get { return _sex; }
            set
            {
                if (value == _sex)
                    return;

                _sex = value;

                base.OnPropertyChanged("GetSex");
            }
        }

        /// <summary>
        /// 检查警号
        /// </summary>
        /// <returns></returns>
        private bool CheckNo()
        {
            //检查是否为空
            if (PoliceInfo.no == null || PoliceInfo.no.Trim().Length == 0)
            {
                MessageBox.Show("请填写警号！");

                return false;
            }

            PoliceInfo.no = PoliceInfo.no.Trim();

            //检查no是否已存在
            if (PoliceService.GetOfficerByNo(PoliceInfo.no) != null)
            {
                MessageBox.Show("警号 " + PoliceInfo.no + " 已存在！");

                return false;
            }

            return true;
        }

        /// <summary>
        /// 姓名
        /// </summary>
        /// <returns></returns>
        private bool CheckName()
        {
            //检查是否为空
            if (PoliceInfo.name == null || PoliceInfo.name.Trim().Length == 0)
            {
                MessageBox.Show("请填写警员姓名！");

                return false;
            }

            PoliceInfo.name = PoliceInfo.name.Trim();

            return true;
        }

        /// <summary>
        /// 警员管理命令
        /// </summary>
        public ICommand PoliceManageCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.PoliceManage)
                    );
            }
        }

        /// <summary>
        /// 警员管理页面
        /// </summary>
        public void PoliceManage(Object obj)
        {
            Button btn = (Button)obj;
            Global.currentFrame.Source = new Uri(btn.Tag.ToString(), UriKind.Relative);
        }


        /// <summary>
        /// 性别选中命令
        /// </summary>
        public ICommand SexCheckedCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.SexChecked)
                    );
            }
        }

        /// <summary>
        /// 性别选中 执行
        /// </summary>
        /// <param name="obj"></param>
        public void SexChecked(Object obj)
        {
            RadioButton rb = (RadioButton)obj;

            PoliceInfo.sex = rb.Content.ToString();

            return;
        }

        /// <summary>
        /// 新增警员命令
        /// </summary>
        public ICommand PoliceNewCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.PoliceNew)
                    );
            }
        }

        /// <summary>
        /// 新增警员
        /// </summary>
        public void PoliceNew(Object obj)
        {
            Button btn = (Button)obj;
            Global.currentFrame.Source = new Uri(btn.Tag.ToString(), UriKind.Relative);
        }

        /// <summary>
        /// 新增警员保存命令
        /// </summary>
        public ICommand PoliceNewSaveCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.PoliceNewSave)
                    );
            }
        }

        /// <summary>
        /// 新增警员
        /// </summary>
        public void PoliceNewSave(Object obj)
        {
            PoliceInfo = GetPoliceInfo;

            if (!CheckNo()) return;
            if (!CheckName()) return;

            bool Result = PoliceService.Insert(PoliceInfo);
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

            return;
        }
    }
}
