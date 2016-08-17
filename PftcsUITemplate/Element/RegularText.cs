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
using System.Text.RegularExpressions;

namespace PftcsUITemplate.Element
{
    /// <summary>
    /// TextBox筛选选项
    /// </summary>
    [Flags]
    public enum StatusModel
    {
        /// <summary>
        /// 正常,只是前面不能输入空格
        /// </summary>
        Normal=0,
        /// <summary>
        /// 纯数字控件
        /// </summary>
        PureDigital=1,
        /// <summary>
        /// 用户名控件
        /// </summary>
        UserName=2
    }
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
    ///     <MyNamespace:RegularText/>
    ///
    /// </summary>
    public class RegularText : TextBox
    {
        public RegularText()
        {
            this.TextChanged += new TextChangedEventHandler(NumercText_TextChanged);
            this.KeyDown += new KeyEventHandler(NumercText_KeyDown);
        }


        /// <summary>
        /// 最大小数点位数
        /// </summary>
        public StatusModel Type
        {
            get { return (StatusModel)GetValue(IsPaddingProperty); }
            set { SetValue(IsPaddingProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsPadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPaddingProperty =
            DependencyProperty.Register("Type", typeof(StatusModel), typeof(RegularText), new PropertyMetadata(StatusModel.Normal));

        private void NumercText_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Type==StatusModel.PureDigital)
            {
                TextBox txt = sender as TextBox;

                //屏蔽非法按键
                if (e.Key == Key.ImeProcessed)
                {
                    e.Handled = false;
                }
                else if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                {
                    e.Handled = false;
                }
                else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.OemPeriod) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }            
        }

        private void NumercText_TextChanged(object sender, TextChangedEventArgs e)
        {
            //屏蔽中文输入和非法字符粘贴输入
            try
            {
                TextBox textBox = sender as TextBox;
                TextChange[] change = new TextChange[e.Changes.Count];
                e.Changes.CopyTo(change, 0);

                int offset = change[0].Offset;
                if (change[0].AddedLength > 0)
                {
                    if (Type == StatusModel.UserName)
                    {
                        if (!Regular_Expressions(textBox.Text))
                        {
                            textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                            textBox.Select(offset, 0);
                            return;
                        }
                    }
                    else if (Type == StatusModel.Normal)
                    {
                        if (!Regular_Expressions_other(textBox.Text))
                        {
                            textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                            textBox.Select(offset, 0);
                        }
                    }
                    else if (Type == StatusModel.PureDigital)
                    {
                        foreach (char w in textBox.Text)
                        {
                            if (w < '0' || w > '9')
                            {
                                textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                                textBox.Select(offset, 0);
                                return;
                            }
                        }
                    }
                }
            }
            catch 
            {
            	
            }
        }


        //        ^(?!_)(?!.*?_$)[a-zA-Z0-9_\u4e00-\u9fa5]+$  其中： 
        //^  与字符串开始的地方匹配 
        //(?!_) 不能以_开头 
        //(?!.*?_$) 不能以_结尾 
        //[a-zA-Z0-9_\u4e00-\u9fa5]+ 至少一个汉字、数字、字母、下划线 
        //$ 与字符串结束的地方匹配
        /// <summary>
        /// 验证用户名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool Regular_Expressions(string str)
        {
            if (Regex.IsMatch(str, @"^(?!_)[_a-zA-Z0-9\u4e00-\u9fa5]+$"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 验证别的输入框，只限制最开始不能输入为空的
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool Regular_Expressions_other(string str)
        {
            if (Regex.IsMatch(str, @"^(?! )"))
            {
                return true;
            }
            return false;
        }
    }
}
