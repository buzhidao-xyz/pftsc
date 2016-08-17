using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using PftcsUITemplate.Util;
using System.Windows.Input;
using System.Windows.Media;

namespace PftcsUITemplate.Controls
{
    public class WindowTemplet : Window
    {

        private int _moveHeight = 30;
        public int MoveHeight
        {
            get { return _moveHeight; }
            set { _moveHeight = value; }
        }

        public WindowTemplet()
        {
            this.DefaultStyleKey = typeof(WindowTemplet);

            //缩放，最大化修复
            WindowBehaviorHelper wh = new WindowBehaviorHelper(this);
            wh.RepairBehavior();

            this.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(WindowTemplet_MouseLeftButtonDown);
            this.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(WindowTemplet_MouseDoubleClick);
        }

        void WindowTemplet_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (e.ChangedButton != MouseButton.Left) return;
            //if (this.ResizeMode == ResizeMode.NoResize) return;

            ////判断点击的控件不是Grid类型，就可以双击
            //if (!(e.OriginalSource is System.Windows.Controls.Image))
            //{
            //    return;
            //}


            //IInputElement feSource = e.MouseDevice.DirectlyOver;


            //Point pt = e.MouseDevice.GetPosition(this);
            //if (pt.Y < MoveHeight)
            //{
            //    if (this.WindowState == WindowState.Normal)
            //        this.WindowState = WindowState.Maximized;
            //    else if (this.WindowState == WindowState.Maximized)
            //        this.WindowState = WindowState.Normal;
            //}
        }

        void WindowTemplet_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point pt = e.MouseDevice.GetPosition(this);
            if (pt.Y < MoveHeight && pt.Y > 0)
                this.DragMove();
        }
    }
}
