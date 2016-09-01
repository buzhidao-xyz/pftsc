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
        private List<officer> PoliceList;
        //警员信息
        private officer PoliceInfo;

        //性别是否选中默认
        private bool Sex_Male = true;
        private bool Sex_FeMale = false;

        public PoliceManagerViewModel()
        {
            PoliceService = new OfficerService();

            PoliceInfo = new officer();
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
        public List<officer> GetPoliceList
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
        public officer GetPoliceInfo
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

        public bool SexIsChecked_Male
        {
            get { return Sex_Male; }
            set
            {
                if (value == Sex_Male)
                    return;

                Sex_Male = value;

                base.OnPropertyChanged("SexIsChecked_Male");
            }
        }

        public bool SexIsChecked_FeMale
        {
            get { return Sex_FeMale; }
            set
            {
                if (value == Sex_FeMale)
                    return;

                Sex_FeMale = value;

                base.OnPropertyChanged("SexIsChecked_FeMale");
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
            if (PoliceService.GetOfficerByNo(PoliceInfo.no, PoliceInfo.id) != null)
            {
                MessageBox.Show("警号 " + PoliceInfo.no + " 已存在！");

                return false;
            }

            return true;
        }

        /// <summary>
        /// 检查性别
        /// </summary>
        /// <returns></returns>
        private bool CheckSex()
        {
            if (PoliceInfo.sex == null) PoliceInfo.sex = "男";

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
            PoliceInfo = new officer();

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
            if (!CheckSex()) return;

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

        /// <summary>
        /// 编辑警员命令
        /// </summary>
        public ICommand PoliceUpCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.PoliceUp)
                    );
            }
        }

        /// <summary>
        /// 新增警员
        /// </summary>
        public void PoliceUp(Object obj)
        {
            Button btn = (Button)obj;
            Global.currentFrame.Source = new Uri(btn.Tag.ToString(), UriKind.Relative);

            if (PoliceInfo.sex == "女")
            {
                SexIsChecked_Male = false;
                Sex_FeMale = true;
            }
            else
            {
                SexIsChecked_Male = true;
                Sex_FeMale = false;
            }

        }

        /// <summary>
        /// 编辑警员保存命令
        /// </summary>
        public ICommand PoliceUpSaveCommand
        {
            get
            {
                return new RelayCommand(
                    new Action<Object>(this.PoliceUpSave)
                    );
            }
        }

        /// <summary>
        /// 编辑警员保存
        /// </summary>
        public void PoliceUpSave(Object obj)
        {
            PoliceInfo = GetPoliceInfo;

            if (!CheckNo()) return;
            if (!CheckName()) return;
            if (!CheckSex()) return;

            bool Result = PoliceService.UpPoliceByID(PoliceInfo);
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
