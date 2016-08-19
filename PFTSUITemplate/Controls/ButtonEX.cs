using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace PFTSUITemplate.Controls
{
    public class ButtonEX : System.Windows.Controls.Button
    {
        static ButtonEX()
		{
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(typeof(Button)));
		}
    }
}
