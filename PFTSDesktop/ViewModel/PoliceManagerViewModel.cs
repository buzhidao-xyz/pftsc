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
using PFTSDesktop.View.PoliceManager;
using PFTSUITemplate.Controls;

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

            this.initData();
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
                if (value == PoliceInfo || value == null)
                    return;

                PoliceInfo = value;
                if (value.id != 0)
                    OfficerFingerList = PoliceService.GetFingerPrintList(value.id);
                base.OnPropertyChanged("GetPoliceInfo");
            }
        }

        public List<officer_fingerprint> OfficerFingerList { get; set; }
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
            GetPoliceList = PoliceService.GetOfficerList((PageIndex - 1) * PageSize, PageSize);
        }

        private void initData()
        {
            TotalCount = PoliceService.GetOfficerCount();
            if (TotalCount == 0)
            {
                GetPoliceList = new List<officer>();
            }
            else
            {
                GetPoliceList = PoliceService.GetOfficerList((PageIndex - 1) * PageSize, PageSize);
            }
        }
        #endregion

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

            // Button btn = (Button)obj;
            //Global.currentFrame.Source = new Uri(btn.Tag.ToString(), UriKind.Relative);
            PoliceNewDlg dlg = new PoliceNewDlg();
            dlg.ShowDialog();
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


                WindowTemplet window = (WindowTemplet)obj;
                window.Close();

                //刷新列表数据
                this.initData();
                //Button btn = (Button)obj;
                // Global.currentFrame.NavigationService.Refresh();
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

            PoliceUpDlg dlg = new PoliceUpDlg();
            dlg.ShowDialog();
            //Button btn = (Button)obj;
            //Global.currentFrame.Source = new Uri(btn.Tag.ToString(), UriKind.Relative);



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

            bool Result = PoliceService.UpPoliceByID(PoliceInfo, OfficerFingerList);
            if (Result)
            {
                MessageBox.Show("保存成功！");

                //刷新列表数据
                this.initData();

                WindowTemplet window = (WindowTemplet)obj;
                window.Close();
                //Button btn = (Button)obj;
                //Global.currentFrame.NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("保存失败！");
            }

            return;

        }
    }
}
