using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PftcsUITemplate.Controls;

namespace PftcsUITemplate.Element
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MoollyClient.Element"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MoollyClient.Element;assembly=MoollyClient.Element"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:ImageButtonControl/>
    ///
    /// </summary>
    public class ImageButtonControl : System.Windows.Controls.Button
    {
        static ImageButtonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
            NormalStateProperty = DependencyProperty.Register("NormalState", typeof(string), typeof(ImageButton));
            MouseOverStateProperty = DependencyProperty.Register("MouseOverState", typeof(string), typeof(ImageButton));
            PressedStateProperty = DependencyProperty.Register("PressedState", typeof(string), typeof(ImageButton));
            DisableStateProperty = DependencyProperty.Register("DisableState", typeof(string), typeof(ImageButton));
        }
        public string NormalState
        {
            get { return (string)GetValue(NormalStateProperty); }
            set { SetValue(NormalStateProperty, value); }
        }

        public string MouseOverState
        {
            get { return (string)GetValue(MouseOverStateProperty); }
            set { SetValue(MouseOverStateProperty, value); }
        }

        public string PressedState
        {
            get { return (string)GetValue(PressedStateProperty); }
            set { SetValue(PressedStateProperty, value); }
        }

        public string DisableState
        {
            get { return (string)GetValue(DisableStateProperty); }
            set { SetValue(DisableStateProperty, value); }
        }

        public static DependencyProperty NormalStateProperty;
        public static DependencyProperty MouseOverStateProperty;
        public static DependencyProperty PressedStateProperty;
        public static DependencyProperty DisableStateProperty;
    }
}
