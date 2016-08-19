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
    public class SuspectViewModel : WorkspaceViewModel
    {
        private RelayCommand addSupcetCommand;
        private RelayCommand getSuspectsCommand;
        private ButtonTemplet preBtn;

        /// <summary>
        /// 添加嫌疑犯页面跳转事件
        /// </summary>
        public ICommand AddSupcetCommand
        {
            get
            {
                if (addSupcetCommand == null)
                {
                    addSupcetCommand = new RelayCommand(
                        new Action<Object>(this.AddSupect)
                        );
                }
                return addSupcetCommand;
            }
        }

        public ICommand GetSuspectsCommand
        {
            get
            {
                if (getSuspectsCommand == null)
                {
                    getSuspectsCommand = new RelayCommand(
                        new Action<Object>(this.GetSuspects)
                        );
                }
                return getSuspectsCommand;
            }
        }

        public void AddSupect(Object obj)
        {
            ButtonEX btn = (ButtonEX)obj;
            FrameSource = new Uri(btn.Tag.ToString(), UriKind.Relative);
        }

        public void GetSuspects(Object obj)
        {
            ButtonTemplet btn = (ButtonTemplet)obj;
            if (preBtn != null)
            {
                if (btn == preBtn)
                    return;
                preBtn.SelectEd = false;
            }
            preBtn = btn;
            btn.SelectEd = true;
            MessageWindow.Show("我执行了", "系统提示", MessageWindowButton.OK, MessageWindowIcon.Error);


        }
    }
}
