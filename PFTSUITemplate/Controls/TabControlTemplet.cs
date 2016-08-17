using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace PFTSUITemplate.Controls
{
    public class TabControlTemplet : TabControl
    {
        static TabControlTemplet()
		{
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControlTemplet), new FrameworkPropertyMetadata(typeof(TabControlTemplet)));
		}
    }
}
