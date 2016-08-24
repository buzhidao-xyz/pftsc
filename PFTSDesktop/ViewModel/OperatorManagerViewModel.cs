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

namespace PFTSDesktop.ViewModel
{
    class OperatorManagerViewModel : WorkspaceViewModel
    {
        //操作员serviceobject
        private OperatorService OperatorService;

        private RelayCommand OperatorCommand;

        //操作员列表对象
        private List<@operator> OperatorList;
        //操作员信息
        private @operator OperatorInfo;

        public OperatorManagerViewModel()
        {
            OperatorService = new OperatorService();

            //获取操作员列表
            OperatorList = OperatorService.GetOperatorList();
        }

        /// <summary>
        /// 获取操作员列表
        /// </summary>
        public List<@operator> GetOperatorList
        {
            get { return OperatorList; }
            set
            {
                if (value == OperatorList)
                    return;

                OperatorList = value;

                base.OnPropertyChanged("GetOperatorList");
            }
        }

        public @operator GetOperatorInfo
        {
            get { return OperatorInfo;  }
            set
            {
                if (value == OperatorInfo)
                    return;

                OperatorInfo = value;

                base.OnPropertyChanged("GetOperatorInfo");
            }
        }

        /// <summary>
        /// 新增操作员命令
        /// </summary>
        public ICommand OperatorNewCommand
        {
            get
            {
                if (OperatorCommand == null)
                {
                    OperatorCommand = new RelayCommand(
                        new Action<Object>(this.OperatorNew)
                        );
                }
                return OperatorCommand;
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
        /// 编辑操作员命令
        /// </summary>
        public ICommand OperatorUpCommand
        {
            get
            {
                if (OperatorCommand == null)
                {
                    OperatorCommand = new RelayCommand(
                        new Action<Object>(this.OperatorUp)
                        );
                }
                return OperatorCommand;
            }
        }

        /// <summary>
        /// 编辑操作员
        /// </summary>
        private void OperatorUp(Object obj)
        {
            var OperatorInfo = GetOperatorInfo;

            Console.WriteLine(OperatorInfo.id);
        }
    }
}
