using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace PFTSUITemplate.Controls
{
    public class TabItemTemplet : TabItem
    {
        public static readonly DependencyProperty MyMoverBrushProperty;
		public static readonly DependencyProperty MyEnterBrushProperty;
		public Brush MyMoverBrush
		{
			get
			{
                return base.GetValue(TabItemTemplet.MyMoverBrushProperty) as Brush;
			}
			set
			{
				base.SetValue(TabItemTemplet.MyMoverBrushProperty, value);
			}
		}
		public Brush MyEnterBrush
		{
			get
			{
				return base.GetValue(TabItemTemplet.MyEnterBrushProperty) as Brush;
			}
			set
			{
				base.SetValue(TabItemTemplet.MyEnterBrushProperty, value);
			}
		}
		static TabItemTemplet()
		{
			TabItemTemplet.MyMoverBrushProperty = DependencyProperty.Register("MyMoverBrush", typeof(Brush), typeof(TabItemTemplet), new PropertyMetadata(null));
			TabItemTemplet.MyEnterBrushProperty = DependencyProperty.Register("MyEnterBrush", typeof(Brush), typeof(TabItemTemplet), new PropertyMetadata(null));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItemTemplet), new FrameworkPropertyMetadata(typeof(TabItemTemplet)));
		}
    }
}
