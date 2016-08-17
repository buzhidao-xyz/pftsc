using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace PftcsUITemplate.Controls
{
    public class Button : System.Windows.Controls.Button
    {
        static Button()
		{
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(typeof(Button)));
		}
    }
}
