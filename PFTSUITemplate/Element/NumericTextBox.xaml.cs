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

namespace PFTSUITemplate.Element
{
    public enum NumericTypes{UnsignedInt,UnsignedDecimal};
    /// <summary>
    /// NumericTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class NumericTextBox : TextBox
    {
        private NumericTypes numericType = NumericTypes.UnsignedInt;
        private object myValue = 0;
        /// <summary>
        /// 设置输入的方式
        /// </summary>
        public NumericTypes NumericType 
        {
            get
            {
                return numericType;
            }
            set
            {
                numericType = value;
                switch(numericType)
                {
                    case NumericTypes.UnsignedInt:
                        {
                            UInt32 _value = Convert.ToUInt32(this.Text);
                            Value = _value;
                        }
                        break;
                    case NumericTypes.UnsignedDecimal:
                        {
                            decimal _value = Math.Abs(Convert.ToDecimal(this.Text));
                            Value = _value;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public object Value
        {
            get { return myValue; }
            set
            {
                switch (numericType)
                {
                    case NumericTypes.UnsignedInt:
                        {
                            if(value is uint)
                            {
                                this.Text = value.ToString();
                            }
                            else
                            {
                                throw new Exception("设置错误的值!");
                            }
                        }
                        break;
                    case NumericTypes.UnsignedDecimal:
                        {
                            if ((value is decimal) && (decimal)value >= 0)
                            {
                                this.Text = value.ToString();
                            }
                            else
                            {
                                throw new Exception("设置错误的值!");
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public NumericTextBox()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            //屏蔽非法按键            
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key.ToString() == "Tab" || (numericType == NumericTypes.UnsignedDecimal && e.Key == Key.Decimal))
            {
                if (txt.Text.Contains(".") && e.Key == Key.Decimal)                
                {                    
                    e.Handled = true;                    
                    return;                
                }                
                e.Handled = false;            
            }            
            else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.OemPeriod) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)            
            {
                if ((numericType == NumericTypes.UnsignedDecimal && e.Key == Key.Decimal))
                {
                    if (txt.Text.Contains(".") && e.Key == Key.OemPeriod)
                    {
                        e.Handled = true;
                        return;
                    }
                    e.Handled = false;
                }
                else
                {
                    if (e.Key == Key.OemPeriod)
                    {
                        e.Handled = true;
                        return;
                    }
                    e.Handled = false;
                }
            }            
            else            
            {
                e.Handled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //屏蔽中文输入和非法字符粘贴输入            
            TextBox textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count]; 
            e.Changes.CopyTo(change, 0); 
            int offset = change[0].Offset; 
            if (change[0].AddedLength > 0) 
            {
                if(numericType == NumericTypes.UnsignedDecimal)
                {
                    double num = 0;
                    if (!Double.TryParse(textBox.Text, out num))
                    {
                        textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                        textBox.Select(offset, 0);
                    } 
                }
                else
                {
                    int num = 0;
                    if (!int.TryParse(textBox.Text, out num))
                    {
                        textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                        textBox.Select(offset, 0);
                    } 
                }
            }
        }
    }
}
