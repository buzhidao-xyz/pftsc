using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace PFTSUITemplate.Controls
{
    public class TabControlLeft : TabControl
    {
        static TabControlLeft()
		{
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControlLeft), new FrameworkPropertyMetadata(typeof(TabControlLeft)));
		}
    }
}
