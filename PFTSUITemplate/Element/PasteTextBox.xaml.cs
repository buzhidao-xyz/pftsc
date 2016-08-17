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
    public delegate void TextPasteHander(string text);
    /// <summary>
    /// PasteTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class PasteTextBox : TextBox
    {
        public TextPasteHander PasteHander;
        public PasteTextBox()
        {
            InitializeComponent();
            DataObject.AddPastingHandler(this, Text_Pasting);
        }

        private void Text_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (PasteHander != null)
            {
                string sysClip = Clipboard.GetText();
                PasteHander(sysClip);
            }
        }
    }
}
