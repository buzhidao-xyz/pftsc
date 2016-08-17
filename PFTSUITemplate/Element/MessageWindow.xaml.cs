using System;
using System.Windows;
using System.Windows.Media;
using PFTSUITemplate.Controls;
using System.Collections.Generic;

namespace PFTSUITemplate.Element
{
    public enum MessageWindowResult{OK,Cancel};

    public enum MessageWindowButton{OK,OKCancel};

    public enum MessageWindowIcon { None, Error, Waring, Doubt ,OK};
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : WindowTemplet
    {
        private static List<MessageWindow> AllMessageWindow = new List<MessageWindow>();

        public MessageWindow()
        {
            InitializeComponent();
        }

        private double MeasureTextWidth(string text, double fontSize, string fontFamily)
        {
            FormattedText formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily.ToString()),
                fontSize,
                Brushes.Black);
            return formattedText.WidthIncludingTrailingWhitespace;
        } 

        internal void AdjustWidth()
        {
            double width = MeasureTextWidth(this.lbCaption.Content.ToString(), this.lbCaption.FontSize, this.lbCaption.FontFamily.Source);
            if (width > 220)
            {
                if (width - 220 + 320 > 1000)
                    this.Width = 1000;
                else
                    this.Width = width - 220 + 320;
            }
            else
                this.Width = 320;
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 关闭当前程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageWindowResult.Cancel;
            this.Close();
        }

        public MessageWindowResult Result {get;private set;}

        public static MessageWindowResult Show(string caption)
        {
            if(string.IsNullOrEmpty(caption))
            {
                throw new ArgumentException();
            }
            MessageWindow msg = new MessageWindow();
            msg.lbTitle.Content = "魔力单店系统";
            msg.lbCaption.Content = caption;
            msg.btnCancel.Visibility = Visibility.Hidden;
            msg.btnOk.Margin = new System.Windows.Thickness(0, 0, 18, 0);
            msg.AdjustWidth();
            msg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            AllMessageWindow.Add(msg);
            msg.ShowDialog();
            AllMessageWindow.Remove(msg);
            return msg.Result;
        }

        public static MessageWindowResult Show(string caption,string title)
        {
            if (string.IsNullOrEmpty(caption) || string.IsNullOrEmpty(title))
            {
                throw new ArgumentException();
            }
            MessageWindow msg = new MessageWindow();
            msg.lbTitle.Content = title;
            msg.lbCaption.Content = caption;
            msg.btnCancel.Visibility = Visibility.Hidden;
            msg.btnOk.Margin = new System.Windows.Thickness(0, 0, 18, 0);
            msg.AdjustWidth();
            msg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            AllMessageWindow.Add(msg);
            msg.ShowDialog();
            AllMessageWindow.Remove(msg);
            return msg.Result;
        }

        public static void closeAll()
        {
            if (AllMessageWindow==null||AllMessageWindow.Count==0)
            {
                return;
            }
            foreach(var w in AllMessageWindow)
            {
                w.Close();
            }
        }

        public static MessageWindowResult Show(string caption, string title,MessageWindowButton btnStyle,MessageWindowIcon icon)
        {
            if (string.IsNullOrEmpty(caption) || string.IsNullOrEmpty(title))
            {
                throw new ArgumentException();
            }
            MessageWindow msg = new MessageWindow();
            msg.lbTitle.Content = title;
            msg.lbCaption.Content = caption;
            msg.btnCancel.Visibility = Visibility.Hidden;
            msg.btnOk.Margin = new System.Windows.Thickness(0, 0, 18, 0);
            if(btnStyle == MessageWindowButton.OK)
            {
                msg.btnCancel.Visibility = Visibility.Hidden;
                msg.btnOk.Margin = new System.Windows.Thickness(0, 0, 18, 0);
            }
            else
            {
                msg.btnOk.Visibility = Visibility.Visible;
                msg.btnCancel.Visibility = Visibility.Visible;
                msg.btnOk.Margin = new System.Windows.Thickness(0, 0, 98, 0);
                msg.btnCancel.Margin = new System.Windows.Thickness(0, 0, 18, 0);
            }
            if(icon == MessageWindowIcon.Error)
            {
                msg.imageError.Visibility = Visibility.Visible;
            }
            else if (icon == MessageWindowIcon.Doubt)
            {
                msg.imageDoubt.Visibility = Visibility.Visible;
            }
            else if(icon == MessageWindowIcon.OK)
            {
                msg.imageOk.Visibility = Visibility.Visible;
            }
            else if(icon == MessageWindowIcon.Waring)
            {
                msg.imageWaring.Visibility = Visibility.Visible;
            }
            msg.AdjustWidth();
            msg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            AllMessageWindow.Add(msg);
            msg.ShowDialog();
            AllMessageWindow.Remove(msg);
            return msg.Result;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageWindowResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageWindowResult.Cancel;
            this.Close();
        }
    }
}
